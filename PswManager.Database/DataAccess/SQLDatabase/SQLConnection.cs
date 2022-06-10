using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
using PswManager.Database.Models;
using PswManager.Utils;
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

        public AccountExistsStatus AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return AccountExistsStatus.InvalidName;
            }

            using var heldLock = locker.GetLock(name, 10000);
            if(!heldLock.Obtained) {
                return AccountExistsStatus.UsedElsewhere;
            }

            return AccountExist_NoLock(name) ? 
                AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
        }

        private bool AccountExist_NoLock(string name) {
            using var cmd = queriesBuilder.GetAccountQuery(name);
            using var cnn = cmd.Connection.GetConnection();
            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        public async ValueTask<AccountExistsStatus> AccountExistAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return AccountExistsStatus.InvalidName;
            }

            using var heldLock = await locker.GetLockAsync(name, 10000);
            if(!heldLock.Obtained) {
                return AccountExistsStatus.UsedElsewhere;
            }

            return await AccountExistAsync_NoLock(name).ConfigureAwait(false) ? 
                AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
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

        public Option<DeleterErrorCode> DeleteAccount(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return DeleterErrorCode.InvalidName;
            }

            using var heldLock = locker.GetLock(name, 50);
            if(heldLock.Obtained == false) {
                return DeleterErrorCode.UsedElsewhere;
            }

            if(!AccountExist_NoLock(name)) {
                return DeleterErrorCode.DoesNotExist;
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            using var cnn = cmd.Connection.GetConnection();
            var result = cmd.ExecuteNonQuery() == 1;
            return (result) ? Option.None<DeleterErrorCode>() : DeleterErrorCode.Undefined;
        }

        public async ValueTask<Option<DeleterErrorCode>> DeleteAccountAsync(string name) { 
            if(string.IsNullOrWhiteSpace(name)) {
                return DeleterErrorCode.InvalidName;
                ;
            }

            using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
            if(!heldLock.Obtained) {
                return DeleterErrorCode.UsedElsewhere;
            }

            if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
                return DeleterErrorCode.DoesNotExist;
            }

            using var cmd = queriesBuilder.DeleteAccountQuery(name);
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
            var result = await cmd.ExecuteNonQueryAsync() == 1;
            return (result)? Option.None<DeleterErrorCode>() : DeleterErrorCode.Undefined;
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

        public Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccounts() {
            using var mainLock = locker.GetAllLocks(10000);
            if(mainLock.Obtained == false) {
                return ReaderAllErrorCode.SomeUsedElsewhere;
            }

            return new(GetAccounts());
        }

        public async Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsAsync() {
            using var mainLock = await locker.GetAllLocksAsync(10000).ConfigureAwait(false);
            if(mainLock.Obtained == false) {
                return ReaderAllErrorCode.SomeUsedElsewhere;
            }

            return new(GetAccountsAsync());
        }

        private IEnumerable<NamedAccountOption> GetAccounts() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            using var cnn = cmd.Connection.GetConnection();

            using var reader = cmd.ExecuteReader();
            while(reader.Read()) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return model;
            }
        }

        private async IAsyncEnumerable<NamedAccountOption> GetAccountsAsync() {
            using var cmd = queriesBuilder.GetAllAccountsQuery();
            await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);

            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            while(await reader.ReadAsync().ConfigureAwait(false)) {
                var model = new AccountModel {
                    Name = reader.GetString(0),
                    Password = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                yield return model;
            }
        }

        public Option<EditorErrorCode> UpdateAccount(string name, AccountModel newModel) {
            if(string.IsNullOrWhiteSpace(name)) {
                return EditorErrorCode.InvalidName;
            }

            using var nameLock = locker.GetLock(name);
            if(nameLock.Obtained == false) {
                return EditorErrorCode.UsedElsewhere;
            }

            if(!AccountExist_NoLock(name)) {
                return EditorErrorCode.DoesNotExist;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = locker.GetLock(newModel.Name);
                    if(newModelLock.Obtained == false) {
                        return EditorErrorCode.NewNameUsedElsewhere;
                    }

                    if(AccountExist_NoLock(newModel.Name)) {
                        return EditorErrorCode.NewNameExistsAlready;
                    }
                }

                using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
                using(var cnn = cmd.Connection.GetConnection()) {
                    cmd.ExecuteNonQuery();
                }

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

            if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
                return EditorErrorCode.DoesNotExist;
            }

            NamesLocker.Lock newModelLock = null;
            try {
                if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                    newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                    if(newModelLock.Obtained == false) {
                        return EditorErrorCode.NewNameUsedElsewhere;
                    }

                    if(await AccountExistAsync_NoLock(newModel.Name).ConfigureAwait(false)) {
                        return EditorErrorCode.NewNameExistsAlready;
                    }
                }

                using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
                await using(var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false)) {
                    await cmd.ExecuteNonQueryAsync();
                }

                return Option.None<EditorErrorCode>();

            } finally {
                newModelLock?.Dispose();
            }
        }
    }
}
