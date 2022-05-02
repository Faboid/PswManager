using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using PswManagerAsync.Locks;
using System;

namespace PswManagerDatabase.DataAccess {
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
        private static readonly ConnectionResult cachedInvalidNameResult = new(false, "The given name isn't valid.");
        private static readonly ConnectionResult cachedFailToLockResult = new(false, "The given account is being used elsewhere.");

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var ownedLock = Locker.GetLock(name, 10000);
            if(ownedLock.Obtained == false) {
                throw new TimeoutException($"The lock in {nameof(AccountExist)} has failed to lock for over ten seconds.");
            }
            return AccountExistInternal(name);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return cachedInvalidNameResult;
            }

            using var ownLock = Locker.GetLock(model.Name, 50);
            if(!ownLock.Obtained) {
                return cachedFailToLockResult;
            }

            if(AccountExistInternal(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            return CreateAccountHook(model);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return cachedInvalidNameResult;
            }

            using var ownedLock = await Locker.GetLockAsync(model.Name, 50);
            if(!ownedLock.Obtained) {
                return cachedFailToLockResult;
            }

            if(AccountExistInternal(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            return await CreateAccountHookAsync(model);
        }


        public ConnectionResult DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return cachedInvalidNameResult;
            }

            using var ownedLock = Locker.GetLock(name, 50);
            if(!ownedLock.Obtained) {
                return cachedFailToLockResult;
            }

            if(!AccountExistInternal(name)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            return DeleteAccountHook(name);
        }


        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return new(cachedInvalidNameResult.Success, cachedInvalidNameResult.ErrorMessage);
            }

            using var ownedLock = Locker.GetLock(name, 50);
            if(!ownedLock.Obtained) {
                return new(cachedFailToLockResult.Success, cachedFailToLockResult.ErrorMessage);
            }

            if(!AccountExistInternal(name)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            return GetAccountHook(name);
        }

        public ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts() {
            using var mainLock = Locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return new(false, "Some account is being used elsewhere in a long operation.");
            }

            return GetAllAccountsHook();
        }

        public async Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync() {
            using var mainLock = await Locker.GetAllLocksAsync(10000);
            if(mainLock.Obtained == false) {
                return new(false, "Some account is being used elsewhere in a long operation.");
            }

            return await GetAllAccountsHookAsync();
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return new(cachedInvalidNameResult.Success, cachedInvalidNameResult.ErrorMessage);
            }

            using var oldModelLock = Locker.GetLock(name, 50);
            if(!oldModelLock.Obtained) {
                return new(cachedFailToLockResult.Success, cachedFailToLockResult.ErrorMessage);
            }

            if(!AccountExistInternal(name)) {
                return new(false, "The given account doesn't exist.");
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = Locker.GetLock(newModel.Name, 50);
                    if(!newModelLock.Obtained) {
                        return new(cachedFailToLockResult.Success, cachedFailToLockResult.ErrorMessage);
                    }
                    if(AccountExistInternal(newModel.Name)) {
                        return new(false, "There is already an account with that name.");
                    }
                }
            
                return UpdateAccountHook(name, newModel);
            } finally {
                if(newModelLock != null) {
                    newModelLock.Dispose();
                }
            }
        }

        private bool AccountExistInternal(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return AccountExistHook(name);
        }

        protected abstract bool AccountExistHook(string name);
        protected abstract ConnectionResult CreateAccountHook(AccountModel model);
        protected abstract Task<ConnectionResult> CreateAccountHookAsync(AccountModel model);
        protected abstract ConnectionResult<AccountModel> GetAccountHook(string name);
        protected abstract ConnectionResult<IEnumerable<AccountResult>> GetAllAccountsHook();
        protected abstract Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsHookAsync();
        protected abstract ConnectionResult<AccountModel> UpdateAccountHook(string name, AccountModel newModel);
        protected abstract ConnectionResult DeleteAccountHook(string name);

    }
}
