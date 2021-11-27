using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Cryptography {
    public interface ICryptoAccount {

        public CryptoString GetPassCryptoString();
        public CryptoString GetEmaCryptoString();

        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);


        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

    }
}
