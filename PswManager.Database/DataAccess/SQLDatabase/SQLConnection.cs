﻿using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
using PswManager.Database.Models;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.SQLDatabase {
    internal class SQLConnection : IDataConnection {

        readonly NamesLocker locker = new();
        readonly DatabaseBuilder database;
        readonly QueriesBuilder queriesBuilder;

        internal SQLConnection() : this("PswManagerDB") { }

        internal SQLConnection(string databaseName) {
            database = new DatabaseBuilder(databaseName);
            queriesBuilder = new QueriesBuilder(database.GetConnection());
        }

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var heldLock = locker.GetLock(name, 10000);
            if(!heldLock.Obtained) {
                throw new TimeoutException("The lock to check the account's existence has failed to be taken for ten seconds.");
            }

            return AccountExist_NoLock(name);
        }

        private bool AccountExist_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            using var cnn = cmd.Connection.GetConnection();
            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        public async ValueTask<bool> AccountExistAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var heldLock = await locker.GetLockAsync(name, 10000);
            if(!heldLock.Obtained) {
                throw new TimeoutException("The lock to check the account's existence has failed to be taken for ten seconds.");
            }

            return await AccountExistAsync_NoLock(name).ConfigureAwait(false);
        }

        private async ValueTask<bool> AccountExistAsync_NoLock(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            using var cmd = queriesBuilder.GetAccountQuery(name);
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await reader.ReadAsync().ConfigureAwait(false);
        }

        public Option<CreatorErrorCode> CreateAccount(AccountModel model) {
            if(!model.IsAllValid(out var errorCode)) {
                return errorCode.ToCreatorErrorCode();
            }

            using var heldLock = locker.GetLock(model.Name, 50);
            if(heldLock.Obtained == false) {
                return CreatorErrorCode.UsedElsewhere;
            }

            if(AccountExist_NoLock(model.Name)) {
                return CreatorErrorCode.AccountExistsAlready;
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);
            using var cnn = cmd.Connection.GetConnection();
            var result = cmd.ExecuteNonQuery() == 1;

            return result switch {
                true => Option.None<CreatorErrorCode>(),
                false => CreatorErrorCode.Undefined,
            };
        }

        public async Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model) {
            if(!model.IsAllValid(out var errorCode)) {
                return errorCode.ToCreatorErrorCode();
            }

            using var heldLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
            if(heldLock.Obtained == false) {
                return CreatorErrorCode.UsedElsewhere;
            }

            if(await AccountExistAsync_NoLock(model.Name).ConfigureAwait(false)) {
                return CreatorErrorCode.AccountExistsAlready;
            }

            using var cmd = queriesBuilder.CreateAccountQuery(model);
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;

            return result switch {
                true => Option.None<CreatorErrorCode>(),
                false => CreatorErrorCode.Undefined,
            };
        }

        public ConnectionResult DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!AccountExist_NoLock(name)) {
                return CachedResults.DoesNotExistResult;
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            using var cnn = cmd.Connection.GetConnection();
            var result = cmd.ExecuteNonQuery() == 1;
            return new ConnectionResult(result);
        }

        public async ValueTask<ConnectionResult> DeleteAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
                return CachedResults.DoesNotExistResult;
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            var result = await cmd.ExecuteNonQueryAsync() == 1;
            return new ConnectionResult(result);
        }

        public Option<AccountModel, ReaderErrorCode> GetAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return ReaderErrorCode.InvalidName;
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return ReaderErrorCode.UsedElsewhere;
            }

            return GetAccount_NoLock(name);
        }

        public async ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return ReaderErrorCode.InvalidName;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return ReaderErrorCode.UsedElsewhere;
            }

            return await GetAccountAsync_NoLock(name).ConfigureAwait(false);
        }

        private Option<AccountModel, ReaderErrorCode> GetAccount_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            using var connection = cmd.Connection.GetConnection();
            using var reader = cmd.ExecuteReader();

            if(!reader.HasRows) {
                return ReaderErrorCode.DoesNotExist;
            }

            reader.Read();
            var model = new AccountModel {
                Name = reader.GetString(0),
                Password = reader.GetString(1),
                Email = reader.GetString(2)
            };

            return model;
        }

        private async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            await using var connection = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            if(!reader.HasRows) {
                return ReaderErrorCode.DoesNotExist;
            }

            await reader.ReadAsync().ConfigureAwait(false);
            var model = new AccountModel {
                Name = reader.GetString(0),
                Password = reader.GetString(1),
                Email = reader.GetString(2),
            };

            return model;
        }

        public ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts() {
            using var mainLock = locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return CachedResults.SomeAccountUsedElsewhereResult;
            }

            return new (true, GetAccounts());
        }

        public async Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync() {
            using var mainLock = await locker.GetAllLocksAsync(10000).ConfigureAwait(false);
            if(mainLock.Obtained == false) {
                return CachedResults.SomeAccountUsedElsewhereResultAsync;
            }

            return new(true, GetAccountsAsync());
        }

        private IEnumerable<AccountResult> GetAccounts() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            using var cnn = cmd.Connection.GetConnection();

            using var reader = cmd.ExecuteReader();
            while(reader.Read()) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return new(model.Name, model);
            }
        }

        private async IAsyncEnumerable<AccountResult> GetAccountsAsync() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);

            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while(await reader.ReadAsync().ConfigureAwait(false)) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return new(model.Name, model);
            }
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var nameLock = locker.GetLock(name);
            if(nameLock.Obtained == false) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!AccountExist_NoLock(name)) {
                return CachedResults.DoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = locker.GetLock(newModel.Name);

                    if(AccountExist_NoLock(newModel.Name)) {
                        return CachedResults.NewAccountNameExistsAlreadyResult;
                    }
                }

                using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
                using(var cnn = cmd.Connection.GetConnection()) {
                    cmd.ExecuteNonQuery();
                }

                return GetAccount_NoLock(string.IsNullOrWhiteSpace(newModel.Name)? name : newModel.Name)
                    .Match<ConnectionResult<AccountModel>>(some => new(true, some), error => new(false, error.ToString()), () => new(false));

            } finally {
                newModelLock?.Dispose();
            }
        }

        public async ValueTask<ConnectionResult<AccountModel>> UpdateAccountAsync(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return CachedResults.InvalidNameResult;
            }

            using var nameLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!nameLock.Obtained) {
                return CachedResults.UsedElsewhereResult;
            }

            if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
                return CachedResults.DoesNotExistResult;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);

                    if(await AccountExistAsync_NoLock(newModel.Name).ConfigureAwait(false)) {
                        return CachedResults.NewAccountNameExistsAlreadyResult;
                    }
                }

                using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
                await using(var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false)) {
                    await cmd.ExecuteNonQueryAsync();
                }

                return (await GetAccountAsync_NoLock(string.IsNullOrWhiteSpace(newModel.Name) ? name : newModel.Name))
                    .Match<ConnectionResult<AccountModel>>(some => new(true, some), error => new(false, error.ToString()), () => new(false));

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
