using PswManagerDatabase.Config;
using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.TextFileConnHelper {
    internal class AccountBuilder {

        private readonly IPaths paths;

        public AccountBuilder(IPaths paths) {
            this.paths = paths;
        }

        public void Create(AccountModel account) {
            SingleValue.Create(paths.AccountsFilePath, account.Name);
            SingleValue.Create(paths.PasswordsFilePath, account.Password);
            SingleValue.Create(paths.EmailsFilePath, account.Email);
        }

        public AccountModel Get(int position) {
            AccountModel account = new();
            account.Name = SingleValue.Get(paths.AccountsFilePath, position);
            account.Password = SingleValue.Get(paths.PasswordsFilePath, position);
            account.Email = SingleValue.Get(paths.EmailsFilePath, position);
            return account;
        }

        public List<AccountModel> GetAll() {
            var names = File.ReadAllLines(paths.AccountsFilePath);
            var passwords = File.ReadAllLines(paths.PasswordsFilePath);
            var emails = File.ReadAllLines(paths.EmailsFilePath);

            var accounts = Enumerable
                .Range(0, names.Length)
                .Select(x => new AccountModel(names[x], passwords[x], emails[x]))
                .ToList();

            return accounts;
        }

        public void Edit(int position, AccountModel newValues) {
            SingleValue.Edit(paths.AccountsFilePath, position, newValues.Name);
            SingleValue.Edit(paths.PasswordsFilePath, position, newValues.Password);
            SingleValue.Edit(paths.EmailsFilePath, position, newValues.Email);
        }

        public void Delete(int position) {
            SingleValue.Delete(paths.AccountsFilePath, position);
            SingleValue.Delete(paths.PasswordsFilePath, position);
            SingleValue.Delete(paths.EmailsFilePath, position);
        }

    }
}
