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

        readonly static ConnectionResult<AccountModel> invalidNameResult = new(false, "The given name is not valid.");
        readonly static ConnectionResult<AccountModel> usedElsewhereResult = new(false, "This account is being used elsewhere.");
        readonly static ConnectionResult<AccountModel> doesNotExistResult = new(false, "The given account does not exist.");

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
            using var cnn = cmd.Connection.GetConnection();
            using var reader = cmd.ExecuteReader();
            return reader.Read();
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
            using var cnn = cmd.Connection.GetConnection();
            var result = cmd.ExecuteNonQuery() == 1;
            return new ConnectionResult(result);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return invalidNameResult;
            }

            using var heldLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult;
            }

            if(AccountExist_NoLock(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;
            return new ConnectionResult(result);
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
            using var cnn = cmd.Connection.GetConnection();
            var result = cmd.ExecuteNonQuery() == 1;
            return new ConnectionResult(result);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return usedElsewhereResult;
            }

            return GetAccount_NoLock(name);
        }

        public async ValueTask<ConnectionResult<AccountModel>> GetAccountAsync(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return usedElsewhereResult;
            }

            return await GetAccountAsync_NoLock(name).ConfigureAwait(false);
        }

        private ConnectionResult<AccountModel> GetAccount_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            using var connection = cmd.Connection.GetConnection();
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
        }

        private async Task<ConnectionResult<AccountModel>> GetAccountAsync_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            await using var connection = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            if(!reader.HasRows) {
                return doesNotExistResult;
            }

            await reader.ReadAsync().ConfigureAwait(false);
            var model = new AccountModel {
                Name = reader.GetString(0),
                Password = reader.GetString(1),
                Email = reader.GetString(2),
            };

            return new(true, model);
        }

        public ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts() {
            using var mainLock = locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return new(false, "Some account is being used elsewhere in a long operation.");
            }

            return new (true, GetAccounts());
        }

        public async Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync() {
            using var mainLock = await locker.GetAllLocksAsync(10000).ConfigureAwait(false);
            if(mainLock.Obtained == false) {
                return new(false, "Some account is being used elsewhere in a long operation.");
            }

            return new(true, GetAccountsAsync());
        }

        private IEnumerable<AccountResult> GetAccounts() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            using var cnn = cmd.Connection.GetConnection();

            using var reader = cmd.ExecuteReader();
            while(reader.Read()) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return new(model.Name, model);
            }
        }

        private async IAsyncEnumerable<AccountResult> GetAccountsAsync() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);

            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while(await reader.ReadAsync().ConfigureAwait(false)) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return new(model.Name, model);
            }
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
                using(var cnn = cmd.Connection.GetConnection()) {
                    cmd.ExecuteNonQuery();
                }

                return GetAccount_NoLock(string.IsNullOrWhiteSpace(newModel.Name)? name : newModel.Name);

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
