using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerDatabase.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        readonly DatabaseBuilder database;

        public SQLConnection(string databaseName) {
            database = new DatabaseBuilder(databaseName);
        }

        public bool AccountExist(string name) {
            using var cnn = database.GetConnection();
            using var cmd = QueriesBuilder.GetAccountQuery(name);
            return cnn.Open(() => {
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            using var cnn = database.GetConnection();
            using var cmd = QueriesBuilder.CreateAccountQuery(model);

            return cnn.Open(() => {
                var result = cmd.ExecuteNonQuery() == 1;
                return new ConnectionResult(result);
            });
        }

        public ConnectionResult DeleteAccount(string name) {
            throw new NotImplementedException();
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            throw new NotImplementedException();
        }

        public ConnectionResult<List<AccountModel>> GetAllAccounts() {
            throw new NotImplementedException();
        }

        public IPaths GetPaths() {
            throw new NotImplementedException();
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            throw new NotImplementedException();
        }
    }
}
