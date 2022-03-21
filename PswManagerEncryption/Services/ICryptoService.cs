namespace PswManagerEncryption.Services {
    public interface ICryptoService {

        string Encrypt(string plainText);
        string Decrypt(string cipherText);

    }
}
