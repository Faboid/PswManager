using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.MemoryDatabase; 
internal class MemoryConnection : BaseConnection {

    readonly Dictionary<string, AccountModel> accounts = new();

    protected override AccountExistsStatus AccountExistHook(string name) {
        return accounts.ContainsKey(name) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    protected override ValueTask<AccountExistsStatus> AccountExistHookAsync(string name) {
        return ValueTask.FromResult(accounts.ContainsKey(name) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist);
    }

    protected override Task<Option<CreatorErrorCode>> CreateAccountHookAsync(AccountModel model) {
        accounts.Add(model.Name, model);
        return Task.FromResult(Option.None<CreatorErrorCode>());
    }

    protected override Task<Option<DeleterErrorCode>> DeleteAccountHookAsync(string name) {
        accounts.Remove(name);
        return Option.None<DeleterErrorCode>().AsTask();
    }

    protected override Task<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name) {
        return Task.FromResult<Option<AccountModel, ReaderErrorCode>>(accounts[name]);
    }

    protected override Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsHookAsync() {
        var enumerator = GetAccountsAsync();
        return Task.FromResult(Option.Some<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>(enumerator));
    }

    private async IAsyncEnumerable<NamedAccountOption> GetAccountsAsync() {
        foreach(var account in accounts.Values) {
            yield return await Task.FromResult<NamedAccountOption>(account);
        }
    }

    protected override Task<Option<EditorErrorCode>> UpdateAccountHookAsync(string name, AccountModel newModel) {
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

        return Option.None<EditorErrorCode>().AsTask();
    }

}
