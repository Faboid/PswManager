using PswManager.Encryption.Cryptography;
namespace PswManager.Encryption.Services;

public interface ICryptoServiceFactory {

    ICryptoService GetCryptoService(char[] password);
    ICryptoService GetCryptoService(char[] password, string version);
    ICryptoService GetCryptoService(Key key);
    ICryptoService GetCryptoService(Key key, string version);

}