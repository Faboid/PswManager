using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess {
    public class TextFileConnection : IDataConnection {

        //IPaths

        public void CreateAccount(AccountModel model) {
            throw new NotImplementedException();
        }

        public void DeleteAccount(AccountModel model) {
            throw new NotImplementedException();
        }

        public AccountModel GetAccount(string name) {
            throw new NotImplementedException();
        }

        public List<AccountModel> GetAllAccounts() {
            throw new NotImplementedException();
        }

        public IPaths GetPaths() {
            throw new NotImplementedException();
        }

        public AccountModel UpdateAccount(string name, AccountModel newModel) {
            throw new NotImplementedException();
        }
    }
}
