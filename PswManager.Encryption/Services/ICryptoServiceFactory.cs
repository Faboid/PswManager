using PswManager.Encryption.Cryptography;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace PswManager.Encryption.Services;

public interface ICryptoServiceFactory {

    ICryptoService GetCryptoService(char[] password);
    ICryptoService GetCryptoService(Key key);

}

internal interface ICryptoServiceInternalFactory : ICryptoServiceFactory {
 
    ICryptoService GetCryptoService(char[] password, string version);
    ICryptoService GetCryptoService(Key key, string version);

}