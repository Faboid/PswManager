using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Database.Models.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Wrappers;

/// <summary>
/// Validates all the requests. Will return an errorstatus/code on failure, but will pass through all requests that satisfy the validation.
/// </summary>
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

    public Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        if(!model.IsValid(out var errorCode)) {
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

    public Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult<Option<IAccountModel, ReaderErrorCode>>(ReaderErrorCode.InvalidName);
        }

        return _connection.GetAccountAsync(name);
    }

    public Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
        if(string.IsNullOrWhiteSpace(name)) {
            return Task.FromResult(EditorResponseCode.InvalidName);
        }

        return _connection.UpdateAccountAsync(name, newModel);
    }
}