using PswManager.Async.Locks;
using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Wrappers;

internal class CheckExistenceWrapper : IDBConnection {

    private readonly IDBConnection _connection;

    public CheckExistenceWrapper(IDBConnection connection) {
        _connection = connection;
    }

    public AccountExistsStatus AccountExist(string name) {
        return _connection.AccountExist(name);
    }

    public Task<AccountExistsStatus> AccountExistAsync(string name) {
        return _connection.AccountExistAsync(name);
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(AccountModel model) {
        if(await AccountExistAsync(model.Name) is AccountExistsStatus.Exist or AccountExistsStatus.UsedElsewhere) {
            return CreatorResponseCode.AccountExistsAlready;
        }

        return await _connection.CreateAccountAsync(model);
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        if(await AccountExistAsync(name) is AccountExistsStatus.NotExist or AccountExistsStatus.UsedElsewhere) {
            return DeleterResponseCode.DoesNotExist;
        }
        
        return await _connection.DeleteAccountAsync(name);
    }

    public IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) {
        return _connection.EnumerateAccountsAsync(locker);
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        if(await AccountExistAsync(name) is AccountExistsStatus.NotExist or AccountExistsStatus.UsedElsewhere) {
            return ReaderErrorCode.DoesNotExist;
        }

        return await _connection.GetAccountAsync(name);
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel) {
        if(await AccountExistAsync(name) is AccountExistsStatus.NotExist or AccountExistsStatus.UsedElsewhere) {
            return EditorResponseCode.DoesNotExist;
        }

        if(name != newModel.Name && !string.IsNullOrWhiteSpace(newModel.Name)) {
            if(await AccountExistAsync(newModel.Name) is AccountExistsStatus.Exist or AccountExistsStatus.UsedElsewhere) {
                return EditorResponseCode.NewNameExistsAlready;
            }
        }

        return await _connection.UpdateAccountAsync(name, newModel);
    }
}