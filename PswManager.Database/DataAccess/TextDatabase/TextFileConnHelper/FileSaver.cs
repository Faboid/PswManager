using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Paths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.TextDatabase.TextFileConnHelper;
internal class FileSaver {

    public FileSaver(IPathsBuilder pathsBuilder) {
        directoryPath = pathsBuilder.GetTextDatabaseDirectory();
        Directory.CreateDirectory(directoryPath);
    }

    readonly string directoryPath;
    private string BuildFilePath(string name) => Path.Combine(directoryPath, $"{name}.txt");

    public bool Exists(string name) {
        return File.Exists(BuildFilePath(name));
    }

    /// <summary>
    /// Checks whether a file exists by offloading <see cref="File.Exists(string?)"/> to another thread.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<bool> ExistsAsync(string name) {
        //wrapper to move it to another thread
        //it's not ideal, but the lack of an async overload leaves no choice
        return Task.Run(() => File.Exists(BuildFilePath(name)));
    }

    public async Task CreateAsync(IReadOnlyAccountModel account) {
        var path = BuildFilePath(account.Name);
        var serialized = AccountSerializer.Serialize(account);
        await File.WriteAllLinesAsync(path, serialized).ConfigureAwait(false);
    }

    /// <summary>
    /// Deleted a file in another thread by wrapping <see cref="File.Delete(string)"/> with <see cref="Task.Run(Action)"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task DeleteAsync(string name) {
        var path = BuildFilePath(name);
        return Task.Run(() => File.Delete(path));
    }

    public async Task<AccountModel> GetAsync(string name) {
        var path = BuildFilePath(name);
        var serialized = await File.ReadAllLinesAsync(path).ConfigureAwait(false);
        return AccountSerializer.Deserialize(serialized);
    }

    public IAsyncEnumerable<NamedAccountOption> GetAllAsync(NamesLocker locker) {
        return Directory.GetFiles(directoryPath)
            .Select<string, Task<NamedAccountOption>>(async x => {
                var name = Path.GetFileNameWithoutExtension(x);
                using var nameLock = await locker.GetLockAsync(name, 5000).ConfigureAwait(false);
                if(!nameLock.Obtained) {
                    return (name, ReaderErrorCode.UsedElsewhere);
                }

                //checks if the account has been deleted while waiting
                if(!await ExistsAsync(name).ConfigureAwait(false)) {
                    return (name, ReaderErrorCode.DoesNotExist);
                }

                var values = await File.ReadAllLinesAsync(x).ConfigureAwait(false);
                var account = AccountSerializer.Deserialize(values);
                return account;
            })
            .AsAsyncEnumerable();
    }

    public async Task UpdateAsync(string name, IReadOnlyAccountModel newModel) {
        var path = BuildFilePath(name);
        var values = await File.ReadAllLinesAsync(path);
        OverwriteModel(values, newModel);
        var newPath = BuildFilePath(values[0]);
        await File.WriteAllLinesAsync(newPath, values);

        if(path != newPath) {
            File.Delete(path);
        }
    }

    private static void OverwriteModel(string[] oldAccount, IReadOnlyAccountModel newAccount) {
        if(!string.IsNullOrWhiteSpace(newAccount.Name)) {
            oldAccount[0] = newAccount.Name;
        }
        if(!string.IsNullOrWhiteSpace(newAccount.Password)) {
            oldAccount[1] = newAccount.Password;
        }
        if(!string.IsNullOrWhiteSpace(newAccount.Email)) {
            oldAccount[2] = newAccount.Email;
        }
    }

}
