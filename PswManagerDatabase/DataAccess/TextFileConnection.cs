using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.DataAccess.TextFileConnHelper;
using PswManagerDatabase.Exceptions;
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

        public void CreateAccount(AccountModel model) {
            if(SearchByName(model.Name) != null) {
                throw new AccountExistsAlreadyException();
            }

            File.AppendAllLines(paths.AccountsFilePath, new[] { model.Name });
            File.AppendAllLines(paths.PasswordsFilePath, new[] { model.Password });
            File.AppendAllLines(paths.EmailsFilePath, new[] { model.Email });
        }

        public AccountModel GetAccount(string name) {
            int position = SearchByName(name) ?? throw new InexistentAccountException("The given account doesn't exist.");

            AccountModel output = new();

            output.Name = File.ReadAllLines(paths.AccountsFilePath).Skip(position).Take(1).First();
            output.Password = File.ReadAllLines(paths.PasswordsFilePath).Skip(position).Take(1).First();
            output.Email = File.ReadAllLines(paths.EmailsFilePath).Skip(position).Take(1).First();

            return output;
        }

        public List<AccountModel> GetAllAccounts() {

            var names = File.ReadAllLines(paths.AccountsFilePath);
            var passwords = File.ReadAllLines(paths.PasswordsFilePath);
            var emails = File.ReadAllLines(paths.EmailsFilePath);

            var accounts = Enumerable
                .Range(0, names.Length - 1)
                .Select(x => new AccountModel(names[x], passwords[x], emails[x]))
                .ToList();

            return accounts;
        }

        public AccountModel UpdateAccount(string name, AccountModel newModel) {
            int position = SearchByName(name) ?? throw new InexistentAccountException("The given account doesn't exist.");

            (position < 0 || position > currentLength).IfTrueThrow(new InexistentAccountException("The given number is out of range."));

            EditValue(paths.AccountsFilePath, position, newModel.Name);
            EditValue(paths.PasswordsFilePath, position, newModel.Password);
            EditValue(paths.EmailsFilePath, position, newModel.Email);

            return GetAccount(newModel.Name ?? name);
        }

        private void EditValue(string path, int position, string value) {
            if(value is not null) {
                var list = File.ReadAllLines(path);
                list[position] = value;
                File.WriteAllLines(path, list);

            }
        }

        public void DeleteAccount(string name) {
            int position = SearchByName(name) ?? throw new InexistentAccountException("The given account doesn't exist.");

            DeleteValue(paths.AccountsFilePath, position);
            DeleteValue(paths.PasswordsFilePath, position);
            DeleteValue(paths.EmailsFilePath, position);
        }

        private void DeleteValue(string path, int position) {
            List<string> list = File.ReadAllLines(path).ToList();
            list.RemoveAt(position);
            File.WriteAllLines(path, list);
        }

        public bool AccountExist(string name) {
            return File.Exists(paths.AccountsFilePath) && SearchByName(name) != null;
        }
    }
}
