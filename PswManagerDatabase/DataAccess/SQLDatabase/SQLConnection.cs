using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerDatabase.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        readonly DatabaseBuilder database;
        readonly QueriesBuilder queriesBuilder;

        public SQLConnection(string databaseName) {
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
