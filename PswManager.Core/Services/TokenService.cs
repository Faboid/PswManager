using PswManager.Encryption.Services;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static PswManager.Core.Services.ITokenService;

[assembly: InternalsVisibleTo("PswManager.Core.Tests")]
namespace PswManager.Core.Services;

internal class TokenService : ITokenService {

    private readonly IFileInfo _tokenFile;
    private readonly string _token;

    public TokenService(IFileInfo tokenFile, string token) {
        _tokenFile = tokenFile;
        _token = token;
    }

    public bool IsSet() => _tokenFile.Exists && !string.IsNullOrWhiteSpace(_tokenFile.FileSystem.File.ReadAllText(_tokenFile.FullName));

    public void SetToken(ICryptoService cryptoService) {
        var encryptedToken = cryptoService.Encrypt(_token);
        _tokenFile.Delete();
        using var streamWriter = _tokenFile.CreateText();
        streamWriter.Write(encryptedToken);
    }

    public TokenResult VerifyToken(ICryptoService cryptoService) {

        if(!IsSet()) {
            return TokenResult.Missing;
        }

        try {
            var encryptedToken = _tokenFile.FileSystem.File.ReadAllText(_tokenFile.FullName);
            var decryptedToken = cryptoService.Decrypt(encryptedToken);

            if(decryptedToken == _token) {
                return TokenResult.Success;
            } else {
                return TokenResult.Failure;
            }
        } catch(CryptographicException) {

            return TokenResult.Failure;
        }

    }

}