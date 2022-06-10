using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.TextDatabase.TextFileConnHelper;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.TextDatabase {
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

        public async ValueTask<AccountExistsStatus> AccountExistAsync(string name) { 
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

        public Option<CreatorErrorCode> CreateAccount(AccountModel model) {
            if(!model.IsAllValid(out var errorCode)) {
                return errorCode.ToCreatorErrorCode();
            }

            using var accLock = locker.GetLock(model.Name, 50);
            if(!accLock.Obtained) {
                return CreatorErrorCode.UsedElsewhere; 
            }

            if(fileSaver.Exists(model.Name)) {
                return CreatorErrorCode.AccountExistsAlready;
            }

            fileSaver.Create(model);
            return Option.None<CreatorErrorCode>();
        }

        public async Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model) {
            if(!model.IsAllValid(out var errorCode)) {
                return errorCode.ToCreatorErrorCode();
            }

            using var accLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
            if(!accLock.Obtained) {
                return CreatorErrorCode.UsedElsewhere;
            }

            if(await fileSaver.ExistsAsync(model.Name).ConfigureAwait(false)) {
                return CreatorErrorCode.AccountExistsAlready;
            }

            await fileSaver.CreateAsync(model).ConfigureAwait(false);
            return Option.None<CreatorErrorCode>();
        }

        public Option<DeleterErrorCode> DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return DeleterErrorCode.InvalidName;
            }

            using var accLock = locker.GetLock(name, 50);
            if(!accLock.Obtained) {
                return DeleterErrorCode.UsedElsewhere;
            }

            if(!fileSaver.Exists(name)) {
                return DeleterErrorCode.DoesNotExist;
            }

            fileSaver.Delete(name);
            return Option.None<DeleterErrorCode>();
        }

        public async ValueTask<Option<DeleterErrorCode>> DeleteAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return DeleterErrorCode.InvalidName;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return DeleterErrorCode.UsedElsewhere;
            }

            if(!await fileSaver.ExistsAsync(name).ConfigureAwait(false)) {
                return DeleterErrorCode.DoesNotExist;
            }

            await fileSaver.DeleteAsync(name);
            return Option.None<DeleterErrorCode>();
        }

        public Option<AccountModel, ReaderErrorCode> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return ReaderErrorCode.InvalidName;
            }

            using var accLock = locker.GetLock(name, 50);
            if(!accLock.Obtained) {
                return ReaderErrorCode.UsedElsewhere;
            }

            if(!fileSaver.Exists(name)) {
                return ReaderErrorCode.DoesNotExist;
            }

            var account = fileSaver.Get(name);
            return account;
        }

        public async ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
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

        public Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccounts() {
            var accounts = fileSaver.GetAll(locker);
            return new(accounts);
        }

        public Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsAsync() { 
            var accounts = fileSaver.GetAllAsync(locker);
            return Option.Some<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>(accounts).AsTask();
        }

        public Option<EditorErrorCode> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return EditorErrorCode.InvalidName;
            }

            using var nameLock = locker.GetLock(name, 50);
            if(!nameLock.Obtained) {
                return EditorErrorCode.UsedElsewhere;
            }

            if(!fileSaver.Exists(name)) {
                return EditorErrorCode.DoesNotExist;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = locker.GetLock(newModel.Name, 50);
                    if(!newModelLock.Obtained) {
                        return EditorErrorCode.NewNameUsedElsewhere;
                    }
                    if(fileSaver.Exists(newModel.Name)) {
                        return EditorErrorCode.NewNameExistsAlready;
                    }
                }

                fileSaver.Update(name, newModel);
                return Option.None<EditorErrorCode>();

            } finally {
                newModelLock?.Dispose();
            }

        }

        public async ValueTask<Option<EditorErrorCode>> UpdateAccountAsync(string name, AccountModel newModel) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return EditorErrorCode.InvalidName;
            }

            using var nameLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!nameLock.Obtained) {
                return EditorErrorCode.UsedElsewhere;
            }

            if(!await fileSaver.ExistsAsync(name)) {
                return EditorErrorCode.DoesNotExist;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                    if(!newModelLock.Obtained) {
                        return EditorErrorCode.NewNameUsedElsewhere;
                    }
                    if(await fileSaver.ExistsAsync(newModel.Name)) {
                        return EditorErrorCode.NewNameExistsAlready;
                    }
                }

                await fileSaver.UpdateAsync(name, newModel).ConfigureAwait(false);
                return Option.None<EditorErrorCode>();

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
