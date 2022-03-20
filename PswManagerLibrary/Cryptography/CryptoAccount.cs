﻿namespace PswManagerLibrary.Cryptography {
    public class CryptoAccount : ICryptoAccount {

        public CryptoAccount(string passPassword, string emaPassword) {
            PassCryptoString = new CryptoString(passPassword);
            EmaCryptoString = new CryptoString(emaPassword);
        }

        public CryptoAccount(ICryptoString passCryptoString, ICryptoString emaCryptoString) {
            PassCryptoString = passCryptoString;
            EmaCryptoString = emaCryptoString;
        }

        public ICryptoString PassCryptoString { get; }
        public ICryptoString EmaCryptoString { get; }

        public ICryptoString GetPassCryptoString() => PassCryptoString;
        public ICryptoString GetEmaCryptoString() => EmaCryptoString;


        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email) {
            return (PassCryptoString.Encrypt(password), EmaCryptoString.Encrypt(email));
        }

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail) {
            return (PassCryptoString.Decrypt(encryptedPassword), EmaCryptoString.Decrypt(encryptedEmail));
        }



        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values) => Encrypt(values.password, values.email);

        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values) => Decrypt(values.encryptedPassword, values.encryptedEmail);

    }
}
