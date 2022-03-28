using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System.Collections.Generic;

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

        public ConnectionResult<List<AccountModel>> GetAllAccounts() {

            using var cmd = queriesBuilder.GetAllAccountsQuery();
            return cmd.Connection.Open(() => {
                using var reader = cmd.ExecuteReader();

                List<AccountModel> accounts = new();

                while(reader.Read()) {
                    accounts.Add(new AccountModel {
                        Name = reader.GetString(0),
                        Password = reader.GetString(1),
                        Email = reader.GetString(2)
                    });
                }

                return new ConnectionResult<List<AccountModel>>(true, accounts);
            });
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
