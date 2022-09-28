using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.JsonDatabase;
internal class JsonConnection : IDBConnection {

    internal JsonConnection() : this("Json") { }

    internal JsonConnection(string customFolderName) {
        directoryPath = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", customFolderName);
        if(!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }
    }

    readonly string directoryPath;
    private string BuildFilePath(string name) {
        return Path.Combine(directoryPath, $"{name}.json");
    }

    private static void OverWriteOldModel(AccountModel oldAccount, IReadOnlyAccountModel newAccount) {
        if(!string.IsNullOrWhiteSpace(newAccount.Name)) {
            oldAccount.Name = newAccount.Name;
        }
        if(!string.IsNullOrWhiteSpace(newAccount.Password)) {
            oldAccount.Password = newAccount.Password;
        }
        if(!string.IsNullOrWhiteSpace(newAccount.Email)) {
            oldAccount.Email = newAccount.Email;
        }
    }



    public AccountExistsStatus AccountExist(string name) {
        return File.Exists(BuildFilePath(name)) ?
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<AccountExistsStatus> AccountExistAsync(string name) {
        return await Task.Run(() => File.Exists(BuildFilePath(name))).ConfigureAwait(false) ?
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        var path = BuildFilePath(model.Name);
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
        return CreatorResponseCode.Success;
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel) {
        var path = BuildFilePath(name);
        AccountModel model;
        using(var readStream = new FileStream(path, FileMode.Open)) {
            model = await JsonSerializer.DeserializeAsync<AccountModel>(readStream).ConfigureAwait(false);
        }
        OverWriteOldModel(model, newModel);
        var newPath = BuildFilePath(model.Name);

        using(var stream = new FileStream(newPath, FileMode.Create)) {
            await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
        }

        //if the file name was changed, deleted the older version
        if(path != newPath) {
            File.Delete(path);
        }

        return EditorResponseCode.Success;
    }

    public Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        File.Delete(BuildFilePath(name));
        return DeleterResponseCode.Success.AsTask();
    }

    public async Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name) {
        var path = BuildFilePath(name);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        AccountModel model = await JsonSerializer.DeserializeAsync<AccountModel>(stream).ConfigureAwait(false);
        return model;
    }

    public async IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker) {
        var accounts = Directory.GetFiles(directoryPath)
            .Select(x => Path.GetFileNameWithoutExtension(x))
            .Select(x => (x, GetAccountAsync(x)));
        //todo - use locker to lock when getting each account
        foreach(var account in accounts) {

            yield return (await account.Item2).Match(
                some => new(some),
                error => (account.x, error),
                () => NamedAccountOption.None()
            );
        }
    }
}
