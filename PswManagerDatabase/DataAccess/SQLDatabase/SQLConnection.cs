using PswManagerAsync.Locks;
using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        readonly NamesLocker locker = new();
        readonly DatabaseBuilder database;
        readonly QueriesBuilder queriesBuilder;

        readonly static ConnectionResult invalidNameResult = new(false, "The given name is not valid.");
        readonly static ConnectionResult usedElsewhereResult = new(false, "This account is being used elsewhere.");

        internal SQLConnection() : this("PswManagerDB") { }

        internal SQLConnection(string databaseName) {
            database = new DatabaseBuilder(databaseName);
            queriesBuilder = new QueriesBuilder(database.GetConnection());
        }

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var heldLock = locker.GetLock(name, 10000);
            if(!heldLock.Obtained) {
                throw new TimeoutException("The lock to check the account's existence has failed to be taken for ten seconds.");
            }

            return AccountExist_NoLock(name);
        }

        private bool AccountExist_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            try {
                cmd.Connection.Open();
                using var reader = cmd.ExecuteReader();
                return reader.Read();
            } finally {
                cmd.Connection.Close();
            }
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return invalidNameResult;
            }

            using var heldLock = locker.GetLock(model.Name, 50);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult;
            }

            if(AccountExist_NoLock(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);

            return cmd.Connection.Open(() => {
                var result = cmd.ExecuteNonQuery() == 1;
                return new ConnectionResult(result);
            });
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return invalidNameResult;
            }

            using var heldLock = await locker.GetLockAsync(model.Name, 50);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult;
            }

            if(AccountExist_NoLock(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);

            return await cmd.Connection.OpenAsync(async () => {
                var result = await cmd.ExecuteNonQueryAsync() == 1;
                return new ConnectionResult(result);
            });
        }

        public ConnectionResult DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult;
            }

            if(!AccountExist_NoLock(name)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            return cmd.Connection.Open(() => {
                var result = cmd.ExecuteNonQuery() == 1;
                return new ConnectionResult(result);
            });
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult.ToConnectionResult<AccountModel>();
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult.ToConnectionResult<AccountModel>();
            }

            return GetAccount_NoLock(name);
        }

        private ConnectionResult<AccountModel> GetAccount_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            return cmd.Connection.Open(() => {
                using var reader = cmd.ExecuteReader();

                if(!reader.HasRows) {
                    return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
                }

                reader.Read();
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                return new ConnectionResult<AccountModel>(true, model);
            });
        }

        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {
            using var mainLock = locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return new(false, "Some account is being used elsewhere in a long operation.");
            }

            return new ConnectionResult<IEnumerable<AccountModel>>(true, GetAccounts());
        }

        private IEnumerable<AccountModel> GetAccounts() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            try {
                Debug.WriteLine("Opened the connection.");
                cmd.Connection.Open();

                using var reader = cmd.ExecuteReader();
                while(reader.Read()) {
                    var model = new AccountModel {
                        Name = reader.GetString(0),
                        Password = reader.GetString(1),
                        Email = reader.GetString(2)
                    };

                    Debug.WriteLine($"Returning model: {model}");
                    yield return model;
                }
            } finally {
                if(cmd.Connection.State == System.Data.ConnectionState.Open) {
                    cmd.Connection.Close();
                    Debug.WriteLine("Closed the connection");
                }
            }

            Debug.WriteLine("Exiting the call.");
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult.ToConnectionResult<AccountModel>();
            }

            using var nameLock = locker.GetLock(name);
            if(nameLock.Obtained == false) {
                return usedElsewhereResult.ToConnectionResult<AccountModel>();
            }

            if(!AccountExist_NoLock(name)) {
                return new(false, "The given account doesn't exist.");
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = locker.GetLock(newModel.Name);

                    if(AccountExist_NoLock(newModel.Name)) {
                        return new(false, "There is already an account with that name.");
                    }
                }


                using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
                cmd.Connection.Open(() => {
                    cmd.ExecuteNonQuery();
                });

                return GetAccount_NoLock(string.IsNullOrWhiteSpace(newModel.Name)? name : newModel.Name);

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
