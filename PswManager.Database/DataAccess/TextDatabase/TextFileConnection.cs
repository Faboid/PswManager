using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.TextDatabase.TextFileConnHelper;
using PswManager.Database.Models;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.TextDatabase;
public class TextFileConnection : IDBConnection {

    internal TextFileConnection() {
        fileSaver = new();
    }

    internal TextFileConnection(string customDB) {
        fileSaver = new(customDB);
    }

    readonly FileSaver fileSaver;

    /// <summary>
    /// Checks if the account exists.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    public AccountExistsStatus AccountExist(string name) {
        return fileSaver.Exists(name) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) {
        return await fileSaver.ExistsAsync(name).ConfigureAwait(false) ?
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        await fileSaver.CreateAsync(model).ConfigureAwait(false);
        return CreatorResponseCode.Success;
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        await fileSaver.DeleteAsync(name);
        return DeleterResponseCode.Success;
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        var account = await fileSaver.GetAsync(name);
        return account;
    }

    public IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) {
        return fileSaver.GetAllAsync(locker);
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
        await fileSaver.UpdateAsync(name, newModel).ConfigureAwait(false);
        return EditorResponseCode.Success;
    }
}
