using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.JsonDatabase {
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

        protected override ConnectionResult CreateAccountHook(AccountModel model) {
            var path = BuildFilePath(model.Name);
            var jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(path, jsonString);

            return new ConnectionResult(true);
        }

        protected async override Task<ConnectionResult> CreateAccountHookAsync(AccountModel model) {
            var path = BuildFilePath(model.Name);
            using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, model).ConfigureAwait(false);
            return new ConnectionResult(true);
        }

        protected override ConnectionResult DeleteAccountHook(string name) {
            File.Delete(BuildFilePath(name));

            return new ConnectionResult(true);
        }

        protected override ConnectionResult<AccountModel> GetAccountHook(string name) {
            var jsonString = File.ReadAllText(BuildFilePath(name));
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);

            return new(true, model);
        }

        protected async Task<AccountResult> GetAccountHookAsync(string name) {
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

            if(!string.IsNullOrWhiteSpace(newModel.Name)) {
                model.Name = newModel.Name;
            }
            if(!string.IsNullOrWhiteSpace(newModel.Password)) {
                model.Password = newModel.Password;
            }
            if(!string.IsNullOrWhiteSpace(newModel.Email)) {
                model.Email = newModel.Email;
            }

            var newPath = BuildFilePath(model.Name);
            var newJsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(newPath, newJsonString);

            return GetAccountHook(model.Name);
        }
    }
}
