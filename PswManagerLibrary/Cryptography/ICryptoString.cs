namespace PswManagerLibrary.Cryptography {
    public interface ICryptoString {

        string Encrypt(string plainText);
        string Decrypt(string cipherText);

    }
}
