using PswManagerAsync.Locks;
using PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.TextDatabase {
    public class TextFileConnection : IDataConnection {

        //cached values
        readonly static ConnectionResult<AccountModel> invalidNameResult = new(false, "The given name is not valid.");
        readonly static ConnectionResult<AccountModel> usedElsewhereResult = new(false, "This account is being used elsewhere.");
        readonly static ConnectionResult<AccountModel> accountExistsAlreadyResult = new(false, "The given account exists already.");
        readonly static ConnectionResult<AccountModel> accountDoesNotExistResult = new(false, "The given account does not exist.");

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
        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var accLock = locker.GetLock(name, 5000);
            if(!accLock.Obtained) {
                throw new TimeoutException($"The account {name} is being used elsewhere.");
            }

            return fileSaver.Exists(name);
        }

        public async ValueTask<bool> AccountExistAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var accLock = await locker.GetLockAsync(name, 5000).ConfigureAwait(false);
            if(!accLock.Obtained) {
                throw new TimeoutException($"The account {name} is being used elsewhere.");
            }

            return await fileSaver.ExistsAsync(name).ConfigureAwait(false);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return invalidNameResult;
            }

            using var accLock = locker.GetLock(model.Name, 50);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(fileSaver.Exists(model.Name)) {
                return accountExistsAlreadyResult;
            }

            fileSaver.Create(model);
            return new(true);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return invalidNameResult;
            }

            using var accLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(await fileSaver.ExistsAsync(model.Name).ConfigureAwait(false)) {
                return accountExistsAlreadyResult;
            }

            await fileSaver.CreateAsync(model).ConfigureAwait(false);
            return new(true);
        }

        public ConnectionResult DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var accLock = locker.GetLock(name, 50);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!fileSaver.Exists(name)) {
                return accountDoesNotExistResult;
            }

            fileSaver.Delete(name);
            return new(true);
        }

        public async ValueTask<ConnectionResult> DeleteAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!await fileSaver.ExistsAsync(name).ConfigureAwait(false)) {
                return accountDoesNotExistResult;
            }

            await fileSaver.DeleteAsync(name);
            return new(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var accLock = locker.GetLock(name, 50);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!fileSaver.Exists(name)) {
                return accountDoesNotExistResult;
            }

            var account = fileSaver.Get(name);
            return new(true, account);
        }

        public async ValueTask<ConnectionResult<AccountModel>> GetAccountAsync(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var accLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!await fileSaver.ExistsAsync(name).ConfigureAwait(false)) {
                return accountDoesNotExistResult;
            }

            var account = await fileSaver.GetAsync(name);
            return new(true, account);
        }

        public ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts() {
            var accounts = fileSaver.GetAll(locker);
            return new(true, accounts);
        }

        public Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync() { 
            var accounts = fileSaver.GetAllAsync(locker);
            return Task.FromResult(new ConnectionResult<IAsyncEnumerable<AccountResult>>(true, accounts));
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var nameLock = locker.GetLock(name, 50);
            if(!nameLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!fileSaver.Exists(name)) {
                return accountDoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = locker.GetLock(newModel.Name, 50);
                    if(!newModelLock.Obtained) {
                        return usedElsewhereResult;
                    }
                    if(fileSaver.Exists(newModel.Name)) {
                        return new(false, $"There is already an account called {newModel.Name}.");
                    }
                }

                var newValues = fileSaver.Update(name, newModel);
                return new(true, newValues);

            } finally {
                newModelLock?.Dispose();
            }

        }

        public async ValueTask<ConnectionResult<AccountModel>> UpdateAccountAsync(string name, AccountModel newModel) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            using var nameLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!nameLock.Obtained) {
                return usedElsewhereResult;
            }

            if(!await fileSaver.ExistsAsync(name)) {
                return accountDoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                    if(!newModelLock.Obtained) {
                        return usedElsewhereResult;
                    }
                    if(await fileSaver.ExistsAsync(newModel.Name)) {
                        return new(false, $"There is already an account named {newModel.Name}.");
                    }
                }

                var newValues = await fileSaver.UpdateAsync(name, newModel).ConfigureAwait(false);
                return new(true, newValues);

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
