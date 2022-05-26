﻿using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using PswManager.Async.Locks;
using System;

namespace PswManager.Database.DataAccess {
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

        public async ValueTask<bool> AccountExistAsync(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var ownedLock = await Locker.GetLockAsync(name, 10000).ConfigureAwait(false);
            if(!ownedLock.Obtained) {
                throw new TimeoutException($"The lock in {nameof(AccountExistAsync)} has failed to lock for over ten seconds.");
            }

            return await AccountExistInternalAsync(name).ConfigureAwait(false);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return CachedResults.InvalidNameResult;
            }

            if(string.IsNullOrWhiteSpace(model.Password)) {
                return CachedResults.MissingPasswordResult;
            }

            if(string.IsNullOrWhiteSpace(model.Email)) {
                return CachedResults.MissingEmailResult;
            }

            using var ownLock = Locker.GetLock(model.Name, 50);
            if(!ownLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(AccountExistInternal(model.Name)) {
                return CachedResults.CreateAccountAlreadyExistsResult;
            }

            return CreateAccountHook(model);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return CachedResults.InvalidNameResult;
            }

            if(string.IsNullOrWhiteSpace(model.Password)) {
                return CachedResults.MissingPasswordResult;
            }

            if(string.IsNullOrWhiteSpace(model.Email)) {
                return CachedResults.MissingEmailResult;
            }

            using var ownedLock = await Locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
            if(!ownedLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(await AccountExistInternalAsync(model.Name).ConfigureAwait(false)) {
                return CachedResults.CreateAccountAlreadyExistsResult;
            }

            return await CreateAccountHookAsync(model).ConfigureAwait(false);
        }


        public ConnectionResult DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var ownedLock = Locker.GetLock(name, 50);
            if(!ownedLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!AccountExistInternal(name)) {
                return CachedResults.DoesNotExistResult;
            }

            return DeleteAccountHook(name);
        }

        public async ValueTask<ConnectionResult> DeleteAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var ownedLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!ownedLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!await AccountExistInternalAsync(name).ConfigureAwait(false)) {
                return CachedResults.DoesNotExistResult;
            }

            return await DeleteAccountHookAsync(name).ConfigureAwait(false);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var ownedLock = Locker.GetLock(name, 50);
            if(!ownedLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!AccountExistInternal(name)) {
                return CachedResults.DoesNotExistResult;
            }

            return GetAccountHook(name);
        }

        public async ValueTask<ConnectionResult<AccountModel>> GetAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var ownedLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!ownedLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!await AccountExistInternalAsync(name).ConfigureAwait(false)) {
                return CachedResults.DoesNotExistResult;
            }

            return await GetAccountHookAsync(name).ConfigureAwait(false);
        }

        public ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts() {
            using var mainLock = Locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return CachedResults.SomeAccountUsedElsewhereResult;
            }

            return GetAllAccountsHook();
        }

        public async Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync() {
            using var mainLock = await Locker.GetAllLocksAsync(10000).ConfigureAwait(false);
            if(mainLock.Obtained == false) {
                return CachedResults.SomeAccountUsedElsewhereResultAsync;
            }

            return await GetAllAccountsHookAsync().ConfigureAwait(false);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var oldModelLock = Locker.GetLock(name, 50);
            if(!oldModelLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!AccountExistInternal(name)) {
                return CachedResults.DoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = Locker.GetLock(newModel.Name, 50);
                    if(!newModelLock.Obtained) {
                        return CachedResults.UsedElsewhereResult;
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

        public async ValueTask<ConnectionResult<AccountModel>> UpdateAccountAsync(string name, AccountModel newModel) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var oldModelLock = await Locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!oldModelLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!await AccountExistInternalAsync(name).ConfigureAwait(false)) {
                return CachedResults.DoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = await Locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                    if(!newModelLock.Obtained) {
                        return CachedResults.UsedElsewhereResult;
                    }
                    if(await AccountExistInternalAsync(newModel.Name)) {
                        return new(false, "There is already an account with that name.");
                    }
                }

                return await UpdateAccountHookAsync(name, newModel);
            } finally {
                newModelLock?.Dispose();
            }
        }

        private bool AccountExistInternal(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return AccountExistHook(name);
        }

        private async ValueTask<bool> AccountExistInternalAsync(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return await AccountExistHookAsync(name).ConfigureAwait(false);
        }

        protected abstract bool AccountExistHook(string name);
        protected abstract ValueTask<bool> AccountExistHookAsync(string name);
        protected abstract ConnectionResult CreateAccountHook(AccountModel model);
        protected abstract ValueTask<ConnectionResult> CreateAccountHookAsync(AccountModel model);
        protected abstract ConnectionResult<AccountModel> GetAccountHook(string name);
        protected abstract ValueTask<AccountResult> GetAccountHookAsync(string name);
        protected abstract ConnectionResult<IEnumerable<AccountResult>> GetAllAccountsHook();
        protected abstract Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsHookAsync();
        protected abstract ConnectionResult<AccountModel> UpdateAccountHook(string name, AccountModel newModel);
        protected abstract ValueTask<ConnectionResult<AccountModel>> UpdateAccountHookAsync(string name, AccountModel newModel);
        protected abstract ConnectionResult DeleteAccountHook(string name);
        protected abstract ValueTask<ConnectionResult> DeleteAccountHookAsync(string name);

    }
}