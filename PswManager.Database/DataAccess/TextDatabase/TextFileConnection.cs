using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.TextDatabase.TextFileConnHelper;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.TextDatabase;
public class TextFileConnection : IDataConnection {

    internal TextFileConnection() {
        fileSaver = new();
    }

    internal TextFileConnection(string customDB) {
        fileSaver = new(customDB);
    }

    readonly NamesLocker locker = new();
    readonly FileSaver fileSaver;

    /// <summary>
    /// Checks if the account exists.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    public AccountExistsStatus AccountExist(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        using var accLock = locker.GetLock(name, 5000);
        if(!accLock.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return fileSaver.Exists(name)? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return AccountExistsStatus.InvalidName;
        }

        using var accLock = await locker.GetLockAsync(name, 5000).ConfigureAwait(false);
        if(!accLock.Obtained) {
            return AccountExistsStatus.UsedElsewhere;
        }

        return await fileSaver.ExistsAsync(name).ConfigureAwait(false) ?
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return errorCode.ToCreatorErrorCode();
        }

        using var accLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
        if(!accLock.Obtained) {
            return CreatorResponseCode.UsedElsewhere;
        }

        if(await fileSaver.ExistsAsync(model.Name).ConfigureAwait(false)) {
            return CreatorResponseCode.AccountExistsAlready;
        }

        await fileSaver.CreateAsync(model).ConfigureAwait(false);
        return CreatorResponseCode.Success;
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterResponseCode.InvalidName;
        }

        using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!heldLock.Obtained) {
            return DeleterResponseCode.UsedElsewhere;
        }

        if(!await fileSaver.ExistsAsync(name).ConfigureAwait(false)) {
            return DeleterResponseCode.DoesNotExist;
        }

        await fileSaver.DeleteAsync(name);
        return DeleterResponseCode.Success;
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        using var accLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!accLock.Obtained) {
            return ReaderErrorCode.UsedElsewhere;
        }

        if(!await fileSaver.ExistsAsync(name).ConfigureAwait(false)) {
            return ReaderErrorCode.DoesNotExist;
        }

        var account = await fileSaver.GetAsync(name);
        return account;
    }

    public IAsyncEnumerable<NamedAccountOption> GetAllAccountsAsync() { 
        return fileSaver.GetAllAsync(locker);
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return EditorResponseCode.InvalidName;
        }

        using var nameLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!nameLock.Obtained) {
            return EditorResponseCode.UsedElsewhere;
        }

        if(!await fileSaver.ExistsAsync(name)) {
            return EditorResponseCode.DoesNotExist;
        }

        NamesLocker.Lock newModelLock = null;
        try {
            if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                if(!newModelLock.Obtained) {
                    return EditorResponseCode.NewNameUsedElsewhere;
                }
                if(await fileSaver.ExistsAsync(newModel.Name)) {
                    return EditorResponseCode.NewNameExistsAlready;
                }
            }

            await fileSaver.UpdateAsync(name, newModel).ConfigureAwait(false);
            return EditorResponseCode.Success;

        } finally {
            newModelLock?.Dispose();
        }
    }
}
