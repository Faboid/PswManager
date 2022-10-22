using PswManager.Encryption.Cryptography;
namespace PswManager.Encryption.Services;

public class CryptoServiceFactory : ICryptoServiceFactory {
    
    public ICryptoService GetCryptoService(char[] password) => new CryptoService(password);
    public ICryptoService GetCryptoService(char[] password, string version) => new CryptoService(password, version);
    public ICryptoService GetCryptoService(Key key) => new CryptoService(key);
    public ICryptoService GetCryptoService(Key key, string version) => new CryptoService(key, version);

}