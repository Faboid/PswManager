using PswManagerHelperMethods;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PswManagerLibrary.Storage {

    /// <summary>
    /// A class whose purpose is to connect the three storage files: accountsfile, passwordsfile, and emailsfile.
    /// </summary>
    public class AccountBuilder {

        readonly IPaths paths;

        public AccountBuilder(IPaths paths) {
            this.paths = paths;
        }

        //todo - turn this function into part of the constructor to set up an instance variable
        //(note: this is a possible future optimization. It's not necessary yet)
        public int CurrentLength() {
            int length = 0;

            using(StreamReader reader = new StreamReader(paths.AccountsFilePath)) {
                while(reader.ReadLine() != null) {
                    length++;
                }
            }

            return length;
        }

        /// <summary>
        /// Returns the position of the name. If it doesn't find any, returns null.
        /// </summary>
        public int? Search(string name) {
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

        public (string name, string password, string email) GetOne(int position) {
            (position < 0 || position > CurrentLength()).IfTrueThrow(new InexistentAccountException("The given number is out of range."));

            string name = File.ReadAllLines(paths.AccountsFilePath).Skip(position).Take(1).First();
            string password = File.ReadAllLines(paths.PasswordsFilePath).Skip(position).Take(1).First();
            string email = File.ReadAllLines(paths.EmailsFilePath).Skip(position).Take(1).First();

            return (name, password, email);
        }

        public (string name, string password, string email) GetOne(string name) {

            int position = Search(name) ?? throw new InexistentAccountException("The given account doesn't exist.");

            return GetOne(position);
        }

        public void EditOne(string name, string newName, string newPassword, string newEmail) {
            EditOne(
                Search(name) ?? throw new InexistentAccountException("The given account doesn't exist."), 
                newName, newPassword, newEmail
                );
        }

        public void EditOne(int position, string newName, string newPassword, string newEmail) {
            (position < 0 || position > CurrentLength()).IfTrueThrow(new InexistentAccountException("The given number is out of range."));

            EditValue(paths.AccountsFilePath, position, newName);
            EditValue(paths.PasswordsFilePath, position, newPassword);
            EditValue(paths.EmailsFilePath, position, newEmail);

        }

        private void EditValue(string path, int position, string value) {
            if(value is not null) {
                var list = File.ReadAllLines(path);
                list[position] = value;
                File.WriteAllLines(path, list);

            }
        }

        public void DeleteOne(string name) {
            DeleteOne(Search(name) ?? throw new InexistentAccountException("The given account doesn't exist."));
        }

        private void DeleteOne(int position) {
            DeleteValue(paths.AccountsFilePath, position);
            DeleteValue(paths.PasswordsFilePath, position);
            DeleteValue(paths.EmailsFilePath, position);
        }

        private void DeleteValue(string path, int position) {
            List<string> list = File.ReadAllLines(path).ToList();
            list.RemoveAt(position);
            File.WriteAllLines(path, list);
        }

    }
}
