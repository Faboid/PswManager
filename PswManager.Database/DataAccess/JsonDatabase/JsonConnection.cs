using PswManager.Database.Models;
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

        protected override bool AccountExistHook(string name) {
            return File.Exists(BuildFilePath(name));
        }

        /// <summary>
        /// Checks if the account exists with a <see cref="Task.Run(System.Func{Task?})"/> wrapper of <see cref="File.Exists(string?)"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override async ValueTask<bool> AccountExistHookAsync(string name) {
            return await Task.Run(() => File.Exists(BuildFilePath(name))).ConfigureAwait(false);
        }

        protected override ConnectionResult CreateAccountHook(AccountModel model) {
            var path = BuildFilePath(model.Name);
            var jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(path, jsonString);

            return new ConnectionResult(true);
        }

        protected async override ValueTask<ConnectionResult> CreateAccountHookAsync(AccountModel model) {
            var path = BuildFilePath(model.Name);
            using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
            return new ConnectionResult(true);
        }

        protected override ConnectionResult DeleteAccountHook(string name) {
            File.Delete(BuildFilePath(name));

            return new ConnectionResult(true);
        }

        protected override ValueTask<ConnectionResult> DeleteAccountHookAsync(string name) {
            //there's no overload, and it's expected to be quick anyway, so I've left it to be synchronous
            //might decide to wrap it in a Task.Run()
            File.Delete(BuildFilePath(name));
            return ValueTask.FromResult(new ConnectionResult(true));
        }

        protected override ConnectionResult<AccountModel> GetAccountHook(string name) {
            var jsonString = File.ReadAllText(BuildFilePath(name));
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);

            return new(true, model);
        }

        protected override async ValueTask<AccountResult> GetAccountHookAsync(string name) {
            var path = BuildFilePath(name);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            AccountModel model = await JsonSerializer.DeserializeAsync<AccountModel>(stream).ConfigureAwait(false);
            return new(name, model);
        }

        protected override ConnectionResult<IEnumerable<AccountResult>> GetAllAccountsHook() {
            var accounts = Directory.GetFiles(directoryPath)
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => new AccountResult(x, GetAccountHook(x)));

            return new(true, accounts);
        }

        protected override Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsHookAsync() {
            return Task.FromResult(new ConnectionResult<IAsyncEnumerable<AccountResult>>(true, GetAccountsAsync()));
        }

        private async IAsyncEnumerable<AccountResult> GetAccountsAsync() {
            var accounts = Directory.GetFiles(directoryPath)
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => GetAccountHookAsync(x));

            foreach(var account in accounts) {
                yield return await account.ConfigureAwait(false);
            }
        }

        protected override ConnectionResult<AccountModel> UpdateAccountHook(string name, AccountModel newModel) {
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

            return GetAccountHook(model.Name);
        }

        protected override async ValueTask<ConnectionResult<AccountModel>> UpdateAccountHookAsync(string name, AccountModel newModel) {
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

            return await GetAccountHookAsync(model.Name).ConfigureAwait(false);
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
