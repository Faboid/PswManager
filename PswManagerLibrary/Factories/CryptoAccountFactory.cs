using PswManagerLibrary.Cryptography;

namespace PswManagerLibrary.Factories {
    public class CryptoAccountFactory : ICryptoAccountFactory {

        public ICryptoAccount CreateCryptoAccount(string passPassword, string emaPassword) {
            return new CryptoAccount(passPassword, emaPassword);
        }

    }
}
