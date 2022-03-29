using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerDatabase.DataAccess.MemoryDatabase {
    internal class MemoryConnection : IDataConnection {

        readonly Dictionary<string, AccountModel> accounts = new();

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return accounts.ContainsKey(name);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            accounts.Add(model.Name, model);
            return new ConnectionResult(true);
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            accounts.Remove(name);
            return new ConnectionResult(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!AccountExist(name)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            return new ConnectionResult<AccountModel>(true, accounts[name]);
        }

        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {
            var list = accounts.Values.ToList();
            return new(true, list);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }
            if(name != newModel.Name && AccountExist(newModel.Name)) {
                return new(false, "There is already an account with that name.");
            }

            var account = accounts[name];

            if(!string.IsNullOrWhiteSpace(newModel.Password)) {
                account.Password = newModel.Password;
            }
            if(!string.IsNullOrWhiteSpace(newModel.Email)) {
                account.Email = newModel.Email;
            }

            if(!string.IsNullOrWhiteSpace(newModel.Name)) {
                account.Name = newModel.Name;
                accounts.Remove(name);
                accounts.Add(newModel.Name, account);
            }

            return new(true, account);
        }

    }
}
