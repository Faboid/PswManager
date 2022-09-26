﻿using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.JsonDatabase; 
internal class JsonConnection : BaseConnection {

    internal JsonConnection() : this("Json") { }

    internal JsonConnection(string customFolderName) {
        directoryPath = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", customFolderName);
        if(!Directory.Exists(directoryPath)) { 
            Directory.CreateDirectory(directoryPath);
        }
    }

    readonly string directoryPath;
    private string BuildFilePath(string name) => Path.Combine(directoryPath, $"{name}.json");

    protected override AccountExistsStatus AccountExistHook(string name) {
        return File.Exists(BuildFilePath(name)) ? 
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    /// <summary>
    /// Checks if the account exists with a <see cref="Task.Run(System.Func{Task?})"/> wrapper of <see cref="File.Exists(string?)"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected override async ValueTask<AccountExistsStatus> AccountExistHookAsync(string name) {
        return await Task.Run(() => File.Exists(BuildFilePath(name))).ConfigureAwait(false) ?
            AccountExistsStatus.Exist : AccountExistsStatus.NotExist;
    }

    protected async override Task<Option<CreatorErrorCode>> CreateAccountHookAsync(AccountModel model) {
        var path = BuildFilePath(model.Name);
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
        return Option.None<CreatorErrorCode>();
    }

    protected override Task<Option<DeleterErrorCode>> DeleteAccountHookAsync(string name) {
        File.Delete(BuildFilePath(name));
        return Option.None<DeleterErrorCode>().AsTask();
    }

    protected override async Task<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name) {
        var path = BuildFilePath(name);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        AccountModel model = await JsonSerializer.DeserializeAsync<AccountModel>(stream).ConfigureAwait(false);
        return model;
    }

    protected override Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> GetAllAccountsHookAsync() {
        return Task.FromResult<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>>(new(GetAccountsAsync()));
    }

    private async IAsyncEnumerable<NamedAccountOption> GetAccountsAsync() {
        var accounts = Directory.GetFiles(directoryPath)
            .Select(x => Path.GetFileNameWithoutExtension(x))
            .Select(x => (x, GetAccountHookAsync(x)));

        foreach(var account in accounts) {

            yield return (await account.Item2).Match(
                some => some,
                error => (account.x, error),
                () => NamedAccountOption.None()
            );
        }
    }

    protected override async Task<Option<EditorErrorCode>> UpdateAccountHookAsync(string name, AccountModel newModel) {
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

        return Option.None<EditorErrorCode>();
    }

    private static void OverWriteOldModel(AccountModel oldAccount, AccountModel newAccount) {
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

}
