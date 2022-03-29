using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PswManagerDatabase.DataAccess.JsonDatabase {
    internal class JsonConnection : IDataConnection {

        internal JsonConnection() : this("Json") { }

        internal JsonConnection(string customFolderName) {
            directoryPath = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", customFolderName);
            if(!Directory.Exists(directoryPath)) { 
                Directory.CreateDirectory(directoryPath);
            }
        }

        readonly string directoryPath;
        private string BuildFilePath(string name) => Path.Combine(directoryPath, $"{name}.json");

        public bool AccountExist(string name) {
            if(string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            return File.Exists(BuildFilePath(name));
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            var path = BuildFilePath(model.Name);
            var jsonString = JsonSerializer.Serialize(model);
            File.WriteAllText(path, jsonString);

            return new ConnectionResult(true);
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }

            File.Delete(BuildFilePath(name));

            return new ConnectionResult(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }

            var jsonString = File.ReadAllText(BuildFilePath(name));
            var model = JsonSerializer.Deserialize<AccountModel>(jsonString);

            return new(true, model);
        }

        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {
            var accounts = Directory.GetFiles(directoryPath)
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .Select(x => GetAccount(x).Value);

            return new(true, accounts);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {
            if(!AccountExist(name)) {
                return new(false, "The given account doesn't exist.");
            }
            if(name != newModel.Name && AccountExist(newModel.Name)) {
                return new(false, "There is already an account with that name.");
            }

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
