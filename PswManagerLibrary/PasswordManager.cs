using PswManagerLibrary.Cryptography;
using System;

namespace PswManagerLibrary {

    /// <summary>
    /// Organizes the storing of the encrypted passwords.
    /// </summary>
    public class PasswordManager : IPasswordManager {

        private readonly CryptoString cryptoString;

        public PasswordManager(string password) {
            this.cryptoString = new CryptoString(password);
        }

        public void CreatePassword(AccountModel account) {
            throw new NotImplementedException();
        }

        public AccountModel GetPassword(string name) {
            throw new NotImplementedException();
        }

        public void EditPassword(AccountModel account, string newPassword) {
            throw new NotImplementedException();
        }

        public void DeletePassword(AccountModel account) {
            throw new NotImplementedException();
        }
    }
}
