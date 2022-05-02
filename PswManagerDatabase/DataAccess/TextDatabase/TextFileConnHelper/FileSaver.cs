using PswManagerAsync.Locks;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    internal class FileSaver {

        public FileSaver() : this("TextSaves") { }

        public FileSaver(string customFolderName) {
            if(string.IsNullOrWhiteSpace(customFolderName)) {
                throw new ArgumentException("Given custom folder name is null or white spaces.", nameof(customFolderName));
            }

            directoryPath = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", customFolderName);
            if(!Directory.Exists(directoryPath)) { 
                Directory.CreateDirectory(directoryPath);
            }
        }

        readonly string directoryPath;
        private string BuildFilePath(string name) => Path.Combine(directoryPath, $"{name}.txt");

        public bool Exists(string name) {
            return File.Exists(BuildFilePath(name));
        }

        public void Create(AccountModel account) {
            var path = BuildFilePath(account.Name);
            var serialized = AccountSerializer.Serialize(account);
            File.WriteAllLines(path, serialized);
        }

        public async Task CreateAsync(AccountModel account) {
            var path = BuildFilePath(account.Name);
            var serialized = AccountSerializer.Serialize(account);
            await File.WriteAllLinesAsync(path, serialized);
        }

        public void Delete(string name) {
            var path = BuildFilePath(name);
            File.Delete(path);
        }

        public AccountModel Get(string name) {
            var path = BuildFilePath(name);
            var serialized = File.ReadAllLines(path);
            return AccountSerializer.Deserialize(serialized);
        }

        public IEnumerable<AccountResult> GetAll(NamesLocker locker) {
            return Directory.GetFiles(directoryPath)
                .AsParallel()
                .Select(x => {
                    var name = Path.GetFileNameWithoutExtension(x);
                    using var nameLock = locker.GetLock(name, 5000);
                    if(!nameLock.Obtained) {
                        return new(name, $"The account {name} is being used elsewhere.");
                    }

                    //checks if the account has been deleted while waiting
                    if(!Exists(name)) {
                        return new(name, $"The account {name} has been deleted or edited.");
                    }

                    var values = File.ReadAllLines(x);
                    var account = AccountSerializer.Deserialize(values);
                    return new AccountResult(name, account);
                });
        }

        public async IAsyncEnumerable<AccountResult> GetAllAsync(NamesLocker locker) {
            var tasks = Directory.GetFiles(directoryPath)
                .Select(async x => {
                    var name = Path.GetFileNameWithoutExtension(x);
                    using var nameLock = await locker.GetLockAsync(name, 5000);
                    if(!nameLock.Obtained) {
                        return new(name, $"The account {name} is being used elsewhere.");
                    }

                    //checks if the account has been deleted while waiting
                    if(!Exists(name)) {
                        return new(name, $"The account {name} has been deleted or edited.");
                    }

                    var values = await File.ReadAllLinesAsync(x);
                    var account = AccountSerializer.Deserialize(values);
                    return new AccountResult(name, account);
                });

            foreach(var task in tasks) {
                yield return await task;
            }
        }

        public AccountModel Update(string name, AccountModel newModel) {
            var path = BuildFilePath(name);
            var values = File.ReadAllLines(path);

            bool deleteOldFile = !string.IsNullOrWhiteSpace(newModel.Name) && name != newModel.Name;

            if(!string.IsNullOrWhiteSpace(newModel.Name)) {
                values[0] = newModel.Name;
            }
            if(!string.IsNullOrWhiteSpace(newModel.Password)) {
                values[1] = newModel.Password;
            }
            if(!string.IsNullOrWhiteSpace(newModel.Email)) {
                values[2] = newModel.Email;
            }

            var newPath = BuildFilePath(values[0]);
            File.WriteAllLines(newPath, values);

            if(deleteOldFile) {
                File.Delete(path);
            }

            return Get(values[0]);
        }
    }
}
