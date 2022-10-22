using PswManager.Encryption.Cryptography;
namespace PswManager.Encryption.Services;

public interface ICryptoServiceFactory {

    ICryptoService GetCryptoService(char[] password);
    ICryptoService GetCryptoService(Key key);

}

internal interface ICryptoServiceInternalFactory : ICryptoServiceFactory {
 
    ICryptoService GetCryptoService(char[] password, string version);
    ICryptoService GetCryptoService(Key key, string version);

}