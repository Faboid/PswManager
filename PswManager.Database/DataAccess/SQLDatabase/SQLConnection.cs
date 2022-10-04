using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
using PswManager.Database.Models;
using PswManager.Paths;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.SQLDatabase;
internal class SQLConnection : IDBConnection {

    readonly DatabaseBuilder database;
    readonly QueriesBuilder queriesBuilder;

    internal SQLConnection() : this(new PathsBuilder()) { }

    internal SQLConnection(IPathsBuilder pathsBuilder) {
        database = new DatabaseBuilder(pathsBuilder);
        queriesBuilder = new QueriesBuilder(database.GetConnection());
    }

    public AccountExistsStatus AccountExist(string name) {
        using var cmd = queriesBuilder.GetAccountQuery(name);
        using var cnn = cmd.Connection.GetConnection();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) {
        using var cmd = queriesBuilder.GetAccountQuery(name);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        return await reader.ReadAsync().ConfigureAwait(false) ? AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        using var cmd = queriesBuilder.CreateAccountQuery(model);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;

        return result switch {
            true => CreatorResponseCode.Success,
            false => CreatorResponseCode.Undefined,
        };
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        using var cmd = queriesBuilder.DeleteAccountQuery(name);
        await using var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false);
        var result = await cmd.ExecuteNonQueryAsync() == 1;
        return (result) ? DeleterResponseCode.Success : DeleterResponseCode.Undefined;
    }

    public async Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
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

    public async IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) {
        using var mainLock = await locker.GetAllLocksAsync().ConfigureAwait(false);

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

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
        using var cmd = queriesBuilder.UpdateAccountQuery(name, newModel);
        await using(var cnn = await cmd.Connection.GetConnectionAsync().ConfigureAwait(false)) {
            await cmd.ExecuteNonQueryAsync();
        }

        return EditorResponseCode.Success;
    }

}
