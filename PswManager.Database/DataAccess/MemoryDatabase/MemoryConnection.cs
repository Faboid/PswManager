using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.MemoryDatabase {
    internal class MemoryConnection : BaseConnection {

        readonly Dictionary<string, AccountModel> accounts = new();

        protected override bool AccountExistHook(string name) {
            return accounts.ContainsKey(name);
        }

        protected override ValueTask<bool> AccountExistHookAsync(string name) {
            return ValueTask.FromResult(accounts.ContainsKey(name));
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

        protected override ValueTask<ConnectionResult> DeleteAccountHookAsync(string name) {
            accounts.Remove(name);
            return ValueTask.FromResult(new ConnectionResult(true));
        }

        protected override Option<AccountModel, ReaderErrorCode> GetAccountHook(string name) {
            return accounts[name];
        }

        protected override ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name) {
            return ValueTask.FromResult<Option<AccountModel, ReaderErrorCode>>(accounts[name]);
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

        protected override ValueTask<ConnectionResult<AccountModel>> UpdateAccountHookAsync(string name, AccountModel newModel) {
            var result = UpdateAccountHook(name, newModel);
            return ValueTask.FromResult(result);
        }

    }
}
