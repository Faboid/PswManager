using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerDatabase.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        public bool AccountExist(string name) {
            throw new NotImplementedException();
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            throw new NotImplementedException();
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
