using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess {
    /// <summary>
    /// A skeleton to build databases connections upon without worrying about validation. <br/>
    /// It implements standard validation checks—child classes need only think how to implement the query logic.
    /// <br/><br/>
    /// Important: <see cref="BaseConnection"/> calls <see cref="AccountExist(string)"/> to check for account existence. 
    /// <br/>If the implementation of that call is expensive, it's advised to implement directly <see cref="IDataConnection"/>.
    /// </summary>
    internal abstract class BaseConnection : IDataConnection {

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return AccountExistHook(name);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            return CreateAccountHook(model);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            return await CreateAccountHookAsync(model);
        }


        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            return DeleteAccountHook(name);
        }


        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!AccountExist(name)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            return GetAccountHook(name);
        }


        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {
            return GetAllAccountsHook();
        }


        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }
            if(name != newModel.Name && AccountExist(newModel.Name)) {
                return new(false, "There is already an account with that name.");
            }

            return UpdateAccountHook(name, newModel);
        }

        protected abstract bool AccountExistHook(string name);
        protected abstract ConnectionResult CreateAccountHook(AccountModel model);
        protected abstract Task<ConnectionResult> CreateAccountHookAsync(AccountModel model);
        protected abstract ConnectionResult<AccountModel> GetAccountHook(string name);
        protected abstract ConnectionResult<IEnumerable<AccountModel>> GetAllAccountsHook();
        protected abstract ConnectionResult<AccountModel> UpdateAccountHook(string name, AccountModel newModel);
        protected abstract ConnectionResult DeleteAccountHook(string name);

    }
}
