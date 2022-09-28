using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Wrappers;

internal class ValidationWrapper : IDataConnection {

    private readonly IDataConnection _connection;

    public ValidationWrapper(IDataConnection connection) {
        _connection = connection;
    }

    public AccountExistsStatus AccountExist(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        return _connection.AccountExist(name);
    }

    public Task<AccountExistsStatus> AccountExistAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult(AccountExistsStatus.InvalidName);
        }

        return _connection.AccountExistAsync(name);
    }

    public Task<CreatorResponseCode> CreateAccountAsync(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return Task.FromResult(errorCode.ToCreatorErrorCode());
        }

        return _connection.CreateAccountAsync(model);
    }

    public Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult(DeleterResponseCode.InvalidName);
        }

        return _connection.DeleteAccountAsync(name);
    }

    public IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync() {
        return _connection.EnumerateAccountsAsync();
    }

    public Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult<Option<AccountModel, ReaderErrorCode>>(ReaderErrorCode.InvalidName);
        }

        return _connection.GetAccountAsync(name);
    }

    public Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult(EditorResponseCode.InvalidName);
        }

        return _connection.UpdateAccountAsync(name, newModel);
    }
}