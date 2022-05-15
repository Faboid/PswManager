using PswManager.Encryption.Services;

namespace PswManagerLibrary.Cryptography {
    public interface ICryptoAccount {

        public ICryptoService GetPassCryptoService();
        public ICryptoService GetEmaCryptoService();

        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);


        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

    }
}
