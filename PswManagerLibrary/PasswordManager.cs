using PswManagerLibrary.Cryptography;
using System;

namespace PswManagerLibrary {

    /// <summary>
    /// Organizes the storing of the encrypted passwords.
    /// </summary>
    public class PasswordManager : IPasswordManager {

        private readonly CryptoString password;

        public PasswordManager(string password) {
            this.password = new CryptoString(password);
        }

        public void CreatePassword() {
            throw new NotImplementedException();
        }

        string IPasswordManager.GetPassword() {
            throw new NotImplementedException();
        }

        public void EditPassword() {
            throw new NotImplementedException();
        }

        public void DeletePassword() {
            throw new NotImplementedException();
        }
    }
}
