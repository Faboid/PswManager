using PswManager.Async.Locks;
using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Wrappers;

/// <summary>
/// Ensures that all values in update are valid by calling Get and substituting the invalid ones for those
/// </summary>
internal class EditSimplificationWrapper : IDBConnection {

    private readonly IDBConnection _connection;

    public EditSimplificationWrapper(IDBConnection connection) {
        _connection = connection;
    }

    public AccountExistsStatus AccountExist(string name) => _connection.AccountExist(name);
    public Task<AccountExistsStatus> AccountExistAsync(string name) => _connection.AccountExistAsync(name);
    public Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) => _connection.CreateAccountAsync(model);
    public Task<DeleterResponseCode> DeleteAccountAsync(string name) => _connection.DeleteAccountAsync(name);
    public IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) => _connection.EnumerateAccountsAsync(locker);
    public Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) => _connection.GetAccountAsync(name);
    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {

        var account = await GetAccountAsync(name);

        var option = await account.BindAsync<EditorResponseCode>(
            async some => {

                //to keep it pure, create a new model
                var input = new AccountModel();

                input.Name = string.IsNullOrWhiteSpace(newModel.Name) ? some.Name : newModel.Name;
                input.Password = string.IsNullOrWhiteSpace(newModel.Password) ? some.Password : newModel.Password;
                input.Email = string.IsNullOrWhiteSpace(newModel.Email) ? some.Email : newModel.Email;

                return await _connection.UpdateAccountAsync(name, input);

            }
        );

        return option.OrDefault();
    }
}