namespace PswManagerLibrary.Cryptography {
    public interface ICryptoAccount {

        public ICryptoString GetPassCryptoString();
        public ICryptoString GetEmaCryptoString();

        public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

        public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);


        public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

        public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

    }
}
