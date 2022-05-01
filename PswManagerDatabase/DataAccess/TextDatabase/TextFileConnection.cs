using PswManagerAsync.Locks;
using PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.TextDatabase {
    public class TextFileConnection : IDataConnection {

        //cached values
        readonly static AccountResult invalidNameResult = new(false, "The given name is not valid.");
        readonly static AccountResult usedElsewhereResult = new(false, "This account is being used elsewhere.");
        readonly static AccountResult accountExistsAlreadyResult = new(false, "The given account exists already.");
        readonly static AccountResult accountDoesNotExistResult = new(false, "The given account does not exist.");

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

            using var accLock = await locker.GetLockAsync(model.Name, 50);
            if(!accLock.Obtained) {
                return usedElsewhereResult;
            }

            if(fileSaver.Exists(model.Name)) {
                return accountExistsAlreadyResult;
            }

            await fileSaver.CreateAsync(model);
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

        //todo - turn this into ConnectionResult<IEnumerable<ConnectionResult<AccountModel>>>
        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {

            //as returning empty AccountModel(s) will throw when decrypting,
            //the timeout values are temporarily filtered off.
            var accounts = fileSaver.GetAll(locker)
                .Where(x => x.Success)
                .Select(x => x.Value);

            return new(true, accounts);
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
    }
}
