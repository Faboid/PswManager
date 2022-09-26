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

    public async ValueTask<AccountExistsStatus> AccountExistAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        using var ownedLock = await Locker.GetLockAsync(name, 10000).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return await AccountExistInternalAsync(name).ConfigureAwait(false);
    }

    public Option<CreatorErrorCode> CreateAccount(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return errorCode.ToCreatorErrorCode();
        }

        using var ownLock = Locker.GetLock(model.Name, 50);
        if(!ownLock.Obtained) {
            return CreatorErrorCode.UsedElsewhere;
        }

        if(AccountExistInternal(model.Name) != AccountExistsStatus.NotExist) {
            return CreatorErrorCode.AccountExistsAlready;
        }

        return CreateAccountHook(model);
    }

    public async Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return errorCode.ToCreatorErrorCode();
        }

        using var ownedLock = await Locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return CreatorErrorCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(model.Name).ConfigureAwait(false) != AccountExistsStatus.NotExist) {
            return CreatorErrorCode.AccountExistsAlready;
        }

        return await CreateAccountHookAsync(model).ConfigureAwait(false);
    }


    public Option<DeleterErrorCode> DeleteAccount(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterErrorCode.InvalidName;
        }

        using var ownedLock = Locker.GetLock(name, 50);
        if(!ownedLock.Obtained) {
            return DeleterErrorCode.UsedElsewhere;
        }

        if(AccountExistInternal(name) == AccountExistsStatus.NotExist) {
            return DeleterErrorCode.DoesNotExist;
        }

        return DeleteAccountHook(name);
    }

    public async ValueTask<Option<DeleterErrorCode>> DeleteAccountAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterErrorCode.InvalidName;
        }

        using var ownedLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!ownedLock.Obtained) {
            return DeleterErrorCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(name).ConfigureAwait(false) == AccountExistsStatus.NotExist) {
            return DeleterErrorCode.DoesNotExist;
        }

        return await DeleteAccountHookAsync(name).ConfigureAwait(false);
    }

    public Option<AccountModel, ReaderErrorCode> GetAccount(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        using var ownedLock = Locker.GetLock(name, 50);
        if(!ownedLock.Obtained) {
            return ReaderErrorCode.UsedElsewhere;
        }

        if(AccountExistInternal(name) == AccountExistsStatus.NotExist) {
            return ReaderErrorCode.DoesNotExist;
        }

        return GetAccountHook(name);
    }

    public async ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) { 
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

    public Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccounts() {
        using var mainLock = Locker.GetAllLocks(10000);
        if(mainLock.Obtained == false) {
            return ReaderAllErrorCode.SomeUsedElsewhere;
        }

        return GetAllAccountsHook();
    }

    public async Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsAsync() {
        using var mainLock = await Locker.GetAllLocksAsync(10000).ConfigureAwait(false);
        if(mainLock.Obtained == false) {
            return ReaderAllErrorCode.SomeUsedElsewhere;
        }

        return await GetAllAccountsHookAsync().ConfigureAwait(false);
    }

    public Option<EditorErrorCode> UpdateAccount(string name, AccountModel newModel) {
        if(string.IsNullOrWhiteSpace(name)) {
            return EditorErrorCode.InvalidName;
        }

        using var oldModelLock = Locker.GetLock(name, 50);
        if(!oldModelLock.Obtained) {
            return EditorErrorCode.UsedElsewhere;
        }

        if(AccountExistInternal(name) == AccountExistsStatus.NotExist) {
            return EditorErrorCode.DoesNotExist;
        }

        NamesLocker.Lock newModelLock = null;
        try {
            if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                newModelLock = Locker.GetLock(newModel.Name, 50);
                if(!newModelLock.Obtained) {
                    return EditorErrorCode.NewNameUsedElsewhere;
                }
                if(AccountExistInternal(newModel.Name) != AccountExistsStatus.NotExist) {

                    return EditorErrorCode.NewNameExistsAlready;
                }
            }
        
            return UpdateAccountHook(name, newModel);
        } finally {
            if(newModelLock != null) {
                newModelLock.Dispose();
            }
        }
    }

    public async ValueTask<Option<EditorErrorCode>> UpdateAccountAsync(string name, AccountModel newModel) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return EditorErrorCode.InvalidName;
        }

        using var oldModelLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!oldModelLock.Obtained) {
            return EditorErrorCode.UsedElsewhere;
        }

        if(await AccountExistInternalAsync(name).ConfigureAwait(false) == AccountExistsStatus.NotExist) {
            return EditorErrorCode.DoesNotExist;
        }

        NamesLocker.Lock newModelLock = null;
        try {
            if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                newModelLock = await Locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                if(!newModelLock.Obtained) {
                    return EditorErrorCode.NewNameUsedElsewhere;
                }
                if(await AccountExistInternalAsync(newModel.Name) != AccountExistsStatus.NotExist) {
                    return EditorErrorCode.NewNameExistsAlready;
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
    protected abstract ValueTask<AccountExistsStatus> AccountExistHookAsync(string name);
    protected abstract Option<CreatorErrorCode> CreateAccountHook(AccountModel model);
    protected abstract ValueTask<Option<CreatorErrorCode>> CreateAccountHookAsync(AccountModel model);
    protected abstract Option<AccountModel, ReaderErrorCode> GetAccountHook(string name);
    protected abstract ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name);
    protected abstract Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccountsHook();
    protected abstract Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsHookAsync();
    protected abstract Option<EditorErrorCode> UpdateAccountHook(string name, AccountModel newModel);
    protected abstract ValueTask<Option<EditorErrorCode>> UpdateAccountHookAsync(string name, AccountModel newModel);
    protected abstract Option<DeleterErrorCode> DeleteAccountHook(string name);
    protected abstract ValueTask<Option<DeleterErrorCode>> DeleteAccountHookAsync(string name);

}
