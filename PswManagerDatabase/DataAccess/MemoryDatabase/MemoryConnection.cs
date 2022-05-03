using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.MemoryDatabase {
    internal class MemoryConnection : BaseConnection {

        readonly Dictionary<string, AccountModel> accounts = new();

        protected override bool AccountExistHook(string name) {
            return accounts.ContainsKey(name);
        }

        protected override ConnectionResult CreateAccountHook(AccountModel model) {
            accounts.Add(model.Name, model);
            return new ConnectionResult(true);
        }

        protected override ValueTask<ConnectionResult> CreateAccountHookAsync(AccountModel model) {
            accounts.Add(model.Name, model);
            return ValueTask.FromResult(new ConnectionResult(true));
        }

        protected override ConnectionResult DeleteAccountHook(string name) {
            accounts.Remove(name);
            return new ConnectionResult(true);
        }

        protected override ConnectionResult<AccountModel> GetAccountHook(string name) {
            return new ConnectionResult<AccountModel>(true, accounts[name]);
        }

        protected override ValueTask<AccountResult> GetAccountHookAsync(string name) {
            return ValueTask.FromResult(new AccountResult(name, accounts[name]));
        }

        protected override ConnectionResult<IEnumerable<AccountResult>> GetAllAccountsHook() {
            var list = accounts
                .Values
                .Select(x => new AccountResult(x.Name, x));

            return new(true, list);
        }

        protected override Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsHookAsync() {
            //fake async as it's just reading a list
            async IAsyncEnumerable<AccountResult> GetAccounts() {
                var list = GetAllAccountsHook().Value;
                foreach(var account in list) {
                    yield return await Task.FromResult(account).ConfigureAwait(false);
                }
            }

            return Task.FromResult(new ConnectionResult<IAsyncEnumerable<AccountResult>>(true, GetAccounts()));
        }

        protected override ConnectionResult<AccountModel> UpdateAccountHook(string name, AccountModel newModel) {
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
