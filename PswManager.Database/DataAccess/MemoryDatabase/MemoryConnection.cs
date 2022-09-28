using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.MemoryDatabase;
internal class MemoryConnection : IDBConnection {

    readonly Dictionary<string, AccountModel> accounts = new();

    public AccountExistsStatus AccountExist(string name) {
        return accounts.ContainsKey(name) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public Task<AccountExistsStatus> AccountExistAsync(string name) {
        return Task.FromResult(accounts.ContainsKey(name) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist);
    }

    public Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        accounts.Add(model.Name, new(model.Name, model.Password, model.Email));
        return Task.FromResult(CreatorResponseCode.Success);
    }

    public Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        accounts.Remove(name);
        return DeleterResponseCode.Success.AsTask();
    }

    public async IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) {
        foreach(var account in accounts.Values) {
            yield return await Task.FromResult<NamedAccountOption>(account);
        }
    }

    public Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        return Task.FromResult<Option<IAccountModel, ReaderErrorCode>>(accounts[name]);
    }

    public Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
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

        return EditorResponseCode.Success.AsTask();
    }

}
