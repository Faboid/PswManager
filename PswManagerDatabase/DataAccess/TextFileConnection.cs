using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.DataAccess.TextFileConnHelper;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess {

    public class TextFileConnection : IDataConnection {

        readonly IPaths paths;
        private int currentLength;

        public TextFileConnection(IPaths paths) {
            this.paths = paths;
            RefreshCurrentLength();
        }

        private void RefreshCurrentLength() {
            int length = 0;

            using(StreamReader reader = new StreamReader(paths.AccountsFilePath)) {
                while(reader.ReadLine() != null) {
                    length++;
                }
            }

            currentLength = length;
        }

        /// <summary>
        /// Returns the position of the name. If it doesn't find any, returns null.
        /// </summary>
        private int? SearchByName(string name) {
            int position = 0;

            using(StreamReader reader = new StreamReader(paths.AccountsFilePath)) {
                string current;
                while((current = reader.ReadLine()) != name) {
                    position++;

                    if(current is null) {
                        return null;
                    }
                }
            }

            return position;
        }

        public IPaths GetPaths() {
            return paths;
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            File.AppendAllLines(paths.AccountsFilePath, new[] { model.Name });
            File.AppendAllLines(paths.PasswordsFilePath, new[] { model.Password });
            File.AppendAllLines(paths.EmailsFilePath, new[] { model.Email });

            return new ConnectionResult(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            AccountModel output = new();

            output.Name = File.ReadAllLines(paths.AccountsFilePath).Skip(position).Take(1).First();
            output.Password = File.ReadAllLines(paths.PasswordsFilePath).Skip(position).Take(1).First();
            output.Email = File.ReadAllLines(paths.EmailsFilePath).Skip(position).Take(1).First();

            return new ConnectionResult<AccountModel>(true, output);
        }

        public ConnectionResult<List<AccountModel>> GetAllAccounts() {

            var names = File.ReadAllLines(paths.AccountsFilePath);
            var passwords = File.ReadAllLines(paths.PasswordsFilePath);
            var emails = File.ReadAllLines(paths.EmailsFilePath);

            var accounts = Enumerable
                .Range(0, names.Length - 1)
                .Select(x => new AccountModel(names[x], passwords[x], emails[x]))
                .ToList();

            return new ConnectionResult<List<AccountModel>>(true, accounts);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {

            if(!AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            (position < 0 || position > currentLength).IfTrueThrow(
                new ArgumentOutOfRangeException(
                    nameof(name), 
                    "The given name has been found in an out of range position. The files might be corrupted."
                )
            );

            EditValue(paths.AccountsFilePath, position, newModel.Name);
            EditValue(paths.PasswordsFilePath, position, newModel.Password);
            EditValue(paths.EmailsFilePath, position, newModel.Email);

            return GetAccount(newModel.Name ?? name);
        }

        private static void EditValue(string path, int position, string value) {
            if(value is not null) {
                var list = File.ReadAllLines(path);
                list[position] = value;
                File.WriteAllLines(path, list);

            }
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name, out int position)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            DeleteValue(paths.AccountsFilePath, position);
            DeleteValue(paths.PasswordsFilePath, position);
            DeleteValue(paths.EmailsFilePath, position);

            return new ConnectionResult(true);
        }

        private static void DeleteValue(string path, int position) {
            List<string> list = File.ReadAllLines(path).ToList();
            list.RemoveAt(position);
            File.WriteAllLines(path, list);
        }

        public bool AccountExist(string name) {
            return File.Exists(paths.AccountsFilePath) && SearchByName(name) != null;
        }

        private bool AccountExist(string name, out int position) {
            position = -1;

            if(!File.Exists(paths.AccountsFilePath))
                return false;

            int? temp = SearchByName(name);
            if(temp == null)
                return false;

            position = (int)temp;

            return true;
        }
    }
}
