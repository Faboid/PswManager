using PswManager.Async.Locks;
using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Wrappers;

/// <summary>
/// Ensures that the same account is accessed only once at a time by using a <see cref="NamesLocker"/>. 
/// </summary>
/// <remarks>
/// For <see cref="EnumerateAccountsAsync"/>, which would need a "main lock", it passes through the request with the <see cref="NamesLocker"/> to allow more fine-tuning during the request.
/// </remarks>
internal class ConcurrencyWrapper : IDataConnection {

    private readonly IDBConnection _connection;
    private readonly int _millisecondsWaitTime;
    private readonly NamesLocker _locker = new();

    public ConcurrencyWrapper(IDBConnection connection, int millisecondsWaitTime = 1000) {
        _connection = connection;
        _millisecondsWaitTime = millisecondsWaitTime;
    }

    public AccountExistsStatus AccountExist(string name) {
        using var locker = _locker.GetLock(name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return _connection.AccountExist(name);
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) {
        using var locker = await _locker.GetLockAsync(name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return await _connection.AccountExistAsync(name);
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        using var locker = await _locker.GetLockAsync(model.Name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return CreatorResponseCode.UsedElsewhere;
        }

        return await _connection.CreateAccountAsync(model);
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        using var locker = await _locker.GetLockAsync(name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return DeleterResponseCode.UsedElsewhere;
        }

        return await _connection.DeleteAccountAsync(name);
    }

    public async Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        using var locker = await _locker.GetLockAsync(name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return ReaderErrorCode.UsedElsewhere;
        }

        return await _connection.GetAccountAsync(name);
    }

    public IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync() {
        return _connection.EnumerateAccountsAsync(_locker);
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
        using var locker = await _locker.GetLockAsync(name, _millisecondsWaitTime);
        if(!locker.Obtained) {
            return EditorResponseCode.UsedElsewhere;
        }

        if(name != newModel.Name && !string.IsNullOrWhiteSpace(newModel.Name)) {
            using var newLocker = await _locker.GetLockAsync(newModel.Name, _millisecondsWaitTime);
            if(!newLocker.Obtained) {
                return EditorResponseCode.NewNameUsedElsewhere;
            }
        }

        return await _connection.UpdateAccountAsync(name, newModel);
    }
}