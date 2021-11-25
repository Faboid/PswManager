using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Cryptography {
    public class CryptoAccount {

        public CryptoString passCryptoString { get; }
        public CryptoString emaCryptoString { get; }

        public CryptoAccount(string passPassword, string emaPassword) {
            passCryptoString = new CryptoString(passPassword);
            emaCryptoString = new CryptoString(emaPassword);
        }

        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email) {
            return (passCryptoString.Encrypt(password), emaCryptoString.Encrypt(email));
        }

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail) {
            return (passCryptoString.Decrypt(encryptedPassword), emaCryptoString.Decrypt(encryptedEmail));
        }

    }
}
