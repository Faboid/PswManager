using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.JsonDatabase {
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

        protected override Option<CreatorErrorCode> CreateAccountHook(AccountModel model) {
            var path = BuildFilePath(model.Name);
            var jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(path, jsonString);

            return Option.None<CreatorErrorCode>();
        }

        protected async override ValueTask<Option<CreatorErrorCode>> CreateAccountHookAsync(AccountModel model) {
            var path = BuildFilePath(model.Name);
            using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
            return Option.None<CreatorErrorCode>();
        }

        protected override Option<DeleterErrorCode> DeleteAccountHook(string name) {
            File.Delete(BuildFilePath(name));

            return Option.None<DeleterErrorCode>();
        }

        protected override ValueTask<Option<DeleterErrorCode>> DeleteAccountHookAsync(string name) {
            //there's no overload, and it's expected to be quick anyway, so I've left it to be synchronous
            //might decide to wrap it in a Task.Run()
            File.Delete(BuildFilePath(name));
            return Option.None<DeleterErrorCode>().AsValueTask();
        }

        protected override Option<AccountModel, ReaderErrorCode> GetAccountHook(string name) {
            var jsonString = File.ReadAllText(BuildFilePath(name));
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);
            return model;
        }

        protected override async ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountHookAsync(string name) {
            var path = BuildFilePath(name);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            AccountModel model = await JsonSerializer.DeserializeAsync<AccountModel>(stream).ConfigureAwait(false);
            return model;
        }

        protected override Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GetAllAccountsHook() {
            var accounts = Directory.GetFiles(directoryPath)
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => GetAccountHook(x).Match(
                    some => some, 
                    error => (x, error), 
                    () => NamedAccountOption.None())
                );

            return new(accounts);
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

        protected override Option<EditorErrorCode> UpdateAccountHook(string name, AccountModel newModel) {
            var path = BuildFilePath(name);
            var jsonString = File.ReadAllText(path);
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);
            OverWriteOldModel(model, newModel);
            var newPath = BuildFilePath(model.Name);
            var newJsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(newPath, newJsonString);

            if(path != newPath) {
                File.Delete(path);
            }

            return Option.None<EditorErrorCode>();
        }

        protected override async ValueTask<Option<EditorErrorCode>> UpdateAccountHookAsync(string name, AccountModel newModel) {
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
}
