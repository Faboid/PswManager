using PswManager.Database.Models;
using PswManager.Encryption.Services;

namespace PswManager.Core.Cryptography {
    public interface ICryptoAccount {

        public ICryptoService GetPassCryptoService();
        public ICryptoService GetEmaCryptoService();

        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);


        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

        public AccountModel Encrypt(AccountModel model);
        public AccountModel Decrypt(AccountModel model);

    }
}
