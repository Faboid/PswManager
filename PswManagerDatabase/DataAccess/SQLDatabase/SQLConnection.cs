using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace PswManagerDatabase.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        readonly DatabaseBuilder database;
        readonly QueriesBuilder queriesBuilder;

        internal SQLConnection() : this("PswManagerDB") { }

        internal SQLConnection(string databaseName) {
            database = new DatabaseBuilder(databaseName);
            queriesBuilder = new QueriesBuilder(database.GetConnection());
        }

        public bool AccountExist(string name) {
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
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);

            return cmd.Connection.Open(() => {
                var result = cmd.ExecuteNonQuery() == 1;
                return new ConnectionResult(result);
            });
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            return cmd.Connection.Open(() => {
                var result = cmd.ExecuteNonQuery() == 1;
                return new ConnectionResult(result);
            });
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {

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
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }
            if(name != newModel.Name && AccountExist(newModel.Name)) {
                return new(false, "There is already an account with that name.");
            }

            using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
            cmd.Connection.Open(() => {
                cmd.ExecuteNonQuery();
            });

            return GetAccount(string.IsNullOrWhiteSpace(newModel.Name)? name : newModel.Name);
        }
    }
}
