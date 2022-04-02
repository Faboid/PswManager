using PswManagerEncryption.Cryptography;
using PswManagerEncryption.Services;
using System;
using System.Linq;

namespace PswManagerLibrary.Cryptography {
    public class CryptoAccount : ICryptoAccount {

        public CryptoAccount(char[] passPassword, char[] emaPassword) {
            PassCryptoString = new CryptoService(passPassword);
            EmaCryptoString = new CryptoService(emaPassword);
        }

        public CryptoAccount(Key passKey, Key emaKey) {
            if(Enumerable.SequenceEqual(passKey.Get(), emaKey.Get())) {
                throw new ArgumentException("The given keys must be different.");
            }

            PassCryptoString = new CryptoService(passKey);
            EmaCryptoString = new CryptoService(emaKey);
        }

        public CryptoAccount(ICryptoService passCryptoString, ICryptoService emaCryptoString) {
            PassCryptoString = passCryptoString;
            EmaCryptoString = emaCryptoString;
        }

        public ICryptoService PassCryptoString { get; }
        public ICryptoService EmaCryptoString { get; }

        public ICryptoService GetPassCryptoService() => PassCryptoString;
        public ICryptoService GetEmaCryptoService() => EmaCryptoString;


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
