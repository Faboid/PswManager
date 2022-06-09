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

        protected override Option<CreatorErrorCode> CreateAccountHook(AccountModel model) {
            accounts.Add(model.Name, model);
            return Option.None<CreatorErrorCode>();
        }

        protected override ValueTask<Option<CreatorErrorCode>> CreateAccountHookAsync(AccountModel model) {
            accounts.Add(model.Name, model);
            return ValueTask.FromResult(Option.None<CreatorErrorCode>());
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

        protected override Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccountsHook() {
            var list = accounts
                .Values
                .Select(x => (NamedAccountOption)x);

            return new(list);
        }

        protected override Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsHookAsync() {
            //fake async as it's just reading a list
            Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAccounts() {
                return GetAllAccountsHook()
                    .Bind(
                        x => Option.Some<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>(ToAsyncEnumerable(x))
                    );
            }

            return Task.FromResult(GetAccounts());
        }

        private static async IAsyncEnumerable<NamedAccountOption> ToAsyncEnumerable(IEnumerable<NamedAccountOption> enumerable) {
            foreach(var item in enumerable) {
                yield return await Task.FromResult(item);
            }
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
