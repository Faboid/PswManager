using PswManager.Encryption.Cryptography;
using System;

namespace PswManager.Encryption.Services;

public class CryptoServiceFactory : ICryptoServiceInternalFactory {
    
    public ICryptoService GetCryptoService(char[] password) => new CryptoService(password);
    public ICryptoService GetCryptoService(Key key) => new CryptoService(key);
    ICryptoService ICryptoServiceInternalFactory.GetCryptoService(char[] password, string version) => new CryptoService(password, version);
    ICryptoService ICryptoServiceInternalFactory.GetCryptoService(Key key, string version) => new CryptoService(key, version);
}