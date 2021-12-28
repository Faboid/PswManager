using PswManagerLibrary.Cryptography;

namespace PswManagerLibrary.Factories {
    public interface ICryptoAccountFactory {

        public ICryptoAccount CreateCryptoAccount(string passPassword, string emaPassword);

    }
}
