using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PswManagerLibrary.Storage {

    /// <summary>
    /// Organizes the storing of the encrypted passwords.
    /// </summary>
    public class PasswordManager : IPasswordManager {

        private readonly CryptoString passCryptoString;
        private readonly CryptoString emaCryptoString;
        private readonly IPaths paths;

        public PasswordManager(IPaths paths, string passPassword, string emaPassword) {
            this.paths = paths;
            this.passCryptoString = new CryptoString(passPassword);
            this.emaCryptoString = new CryptoString(emaPassword);
        }

        public void CreatePassword(string name, string password, string email = null) {

            bool existing = false;

            if(File.Exists(paths.AccountsFilePath)) {
                //check for same-named accounts
                existing = File.ReadAllLines(paths.AccountsFilePath).Any(x => x == name);
            }

            if(existing) {
                throw new InvalidCommandException("The account you're trying to create exists already.");
            }

            //create new account
            File.AppendAllLines(paths.AccountsFilePath, new [] { name });

            //create new password
            File.AppendAllLines(paths.PasswordsFilePath, new[] { passCryptoString.Encrypt(password) });

            //create new email
            File.AppendAllLines(paths.EmailsFilePath, new[] { emaCryptoString.Encrypt(email ?? "") });

        }

        public string GetPassword(string name) {
            throw new NotImplementedException();
        }

        public void EditPassword(string name, string oldPassword, string newPassword) {
            throw new NotImplementedException();
        }

        public void DeletePassword(string name, string password) {
            throw new NotImplementedException();
        }
    }
}
