using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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

        protected override ConnectionResult DeleteAccountHook(string name) {
            File.Delete(BuildFilePath(name));

            return new ConnectionResult(true);
        }

        protected override ConnectionResult<AccountModel> GetAccountHook(string name) {
            var jsonString = File.ReadAllText(BuildFilePath(name));
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);

            return new(true, model);
        }

        protected override ConnectionResult<IEnumerable<AccountModel>> GetAllAccountsHook() {
            var accounts = Directory.GetFiles(directoryPath)
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => GetAccount(x).Value);

            return new(true, accounts);
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

            return GetAccount(model.Name);
        }
    }
}
