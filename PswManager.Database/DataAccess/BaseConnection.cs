using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;

namespace PswManager.Database.DataAccess;
/// <summary>
/// A skeleton to build databases connections upon without worrying about validation and locking. <br/>
/// It implements standard validation checks—child classes need only think how to implement the query logic—and
/// locking to make sure the same account can be only called once at a time.
/// <br/><br/>
/// Important points: 
/// <br/>- <see cref="BaseConnection"/> calls <see cref="AccountExist(string)"/> to check for account existence. 
/// If the implementation of that call is expensive, it's advised to implement directly <see cref="IDataConnection"/>.
/// <br/>- Calls to <see cref="GetAllAccounts"/> lock ALL accounts. 
/// Implemening <see cref="IDataConnection"/> directly can allow locking one at a time.
/// </summary>
internal abstract class BaseConnection : IDataConnection {

    //todo - implement IDisposable to clean up NamesLocker
    private readonly NamesLocker Locker = new();

    public AccountExistsStatus AccountExist(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        using var ownedLock = Locker.GetLock(name, 10000);
        if(ownedLock.Obtained == false) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return AccountExistInternal(name);
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        using var ownedLock = await Locker.GetLockAsync(name, 10000).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return await AccountExistInternalAsync(name).ConfigureAwait(false);
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return errorCode.ToCreatorErrorCode();
        }

        using var ownedLock = await Locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return CreatorResponseCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(model.Name).ConfigureAwait(false) != AccountExistsStatus.NotExist) {
            return CreatorResponseCode.AccountExistsAlready;
        }

        return await CreateAccountHookAsync(model).ConfigureAwait(false);
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterResponseCode.InvalidName;
        }

        using var ownedLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return DeleterResponseCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(name).ConfigureAwait(false) == AccountExistsStatus.NotExist) {
            return DeleterResponseCode.DoesNotExist;
        }

        return await DeleteAccountHookAsync(name).ConfigureAwait(false);
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        using var ownedLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return ReaderErrorCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(name).ConfigureAwait(false) == AccountExistsStatus.NotExist) {
            return ReaderErrorCode.DoesNotExist;
        }

        return await GetAccountHookAsync(name).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync() {
        using var mainLock = await Locker.GetAllLocksAsync().ConfigureAwait(false);
        await foreach(var account in GetAllAccountsHookAsync()) {
            yield return account;
        }
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return EditorResponseCode.InvalidName;
        }

        using var oldModelLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!oldModelLock.Obtained) {
            return EditorResponseCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(name).ConfigureAwait(false) == AccountExistsStatus.NotExist) {
            return EditorResponseCode.DoesNotExist;
        }

        NamesLocker.Lock newModelLock = null;
        try {
            if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                newModelLock = await Locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                if(!newModelLock.Obtained) {
                    return EditorResponseCode.NewNameUsedElsewhere;
                }
                if(await AccountExistInternalAsync(newModel.Name) != AccountExistsStatus.NotExist) {
                    return EditorResponseCode.NewNameExistsAlready;
                }
            }

            return await UpdateAccountHookAsync(name, newModel);
        } finally {
            newModelLock?.Dispose();
        }
    }

    private AccountExistsStatus AccountExistInternal(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        return AccountExistHook(name);
    }

    private async ValueTask<AccountExistsStatus> AccountExistInternalAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        return await AccountExistHookAsync(name).ConfigureAwait(false);
    }

    protected abstract AccountExistsStatus AccountExistHook(string name);
    protected abstract Task<AccountExistsStatus> AccountExistHookAsync(string name);
    protected abstract Task<CreatorResponseCode> CreateAccountHookAsync(AccountModel model);
    protected abstract Task<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name);
    protected abstract IAsyncEnumerable<NamedAccountOption> GetAllAccountsHookAsync();
    protected abstract Task<EditorResponseCode> UpdateAccountHookAsync(string name, AccountModel newModel);
    protected abstract Task<DeleterResponseCode> DeleteAccountHookAsync(string name);

}
