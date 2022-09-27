using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.SQLDatabase;
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

    public async Task<AccountExistsStatus> AccountExistAsync(string name) { 
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

    private async Task<bool> AccountExistAsync_NoLock(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return false;
        }

        using var cmd = queriesBuilder.GetAccountQuery(name);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        return await reader.ReadAsync().ConfigureAwait(false);
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(AccountModel model) {
        if(!model.IsAllValid(out var errorCode)) {
            return errorCode.ToCreatorErrorCode();
        }

        using var heldLock = await locker.GetLockAsync(model.Name, 50).ConfigureAwait(false);
        if(heldLock.Obtained == false) {
            return CreatorResponseCode.UsedElsewhere;
        }

        if(await AccountExistAsync_NoLock(model.Name).ConfigureAwait(false)) {
            return CreatorResponseCode.AccountExistsAlready;
        }

        using var cmd = queriesBuilder.CreateAccountQuery(model);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;

        return result switch {
            true => CreatorResponseCode.Success,
            false => CreatorResponseCode.Undefined,
        };
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) { 
        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterResponseCode.InvalidName;
            ;
        }

        using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!heldLock.Obtained) {
            return DeleterResponseCode.UsedElsewhere;
        }

        if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
            return DeleterResponseCode.DoesNotExist;
        }

        using var cmd = queriesBuilder.DeleteAccountQuery(name);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        var result = await cmd.ExecuteNonQueryAsync() == 1;
        return (result)? DeleterResponseCode.Success : DeleterResponseCode.Undefined;
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        using var heldLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!heldLock.Obtained) {
            return ReaderErrorCode.UsedElsewhere;
        }

        return await GetAccountAsync_NoLock(name).ConfigureAwait(false);
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

    public async Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsAsync() {
        using var mainLock = await locker.GetAllLocksAsync(10000).ConfigureAwait(false);
        if(mainLock.Obtained == false) {
            return ReaderAllErrorCode.SomeUsedElsewhere;
        }

        return new(GetAccountsAsync());
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

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel) {
        if(string.IsNullOrWhiteSpace(name)) {
            return EditorResponseCode.InvalidName;
        }

        using var nameLock = await locker.GetLockAsync(name, 50).ConfigureAwait(false);
        if(!nameLock.Obtained) {
            return EditorResponseCode.UsedElsewhere;
        }

        if(!await AccountExistAsync_NoLock(name).ConfigureAwait(false)) {
            return EditorResponseCode.DoesNotExist;
        }

        NamesLocker.Lock newModelLock = null;
        try {
            if(!string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name) {
                newModelLock = await locker.GetLockAsync(newModel.Name, 50).ConfigureAwait(false);
                if(newModelLock.Obtained == false) {
                    return EditorResponseCode.NewNameUsedElsewhere;
                }

                if(await AccountExistAsync_NoLock(newModel.Name).ConfigureAwait(false)) {
                    return EditorResponseCode.NewNameExistsAlready;
                }
            }

            using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
            await using(var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false)) {
                await cmd.ExecuteNonQueryAsync();
            }

            return EditorResponseCode.Success;

        } finally {
            newModelLock?.Dispose();
        }
    }
}
