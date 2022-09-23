using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using PswManager.Utils;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly:InternalsVisibleTo("PswManager.Core.Tests")]
namespace PswManager.Core.Cryptography; 
internal class TokenService {

    private readonly ICryptoService cryptoService;
    private readonly string tokenPath;
    private const string plainText = "This is the correct password.";

    public TokenService(char[] password) : this(new Key(password)) { }

    public TokenService(char[] password, string customPath) : this(new Key(password), customPath) { }

    public TokenService(Key key) : this(key, GetDefaultPath()) { }

    public TokenService(Key key, string customPath) : this(new CryptoService(key), customPath) { }

    internal TokenService(ICryptoService cryptoService) : this(cryptoService, GetDefaultPath()) { }

    internal TokenService(ICryptoService cryptoService, string customPath) {
        tokenPath = customPath;
        this.cryptoService = cryptoService;
    }

    public bool IsTokenSetUp() => File.Exists(tokenPath) && File.ReadAllLines(tokenPath).Length == 1;
    public static bool IsTokenSetUp(string path) => File.Exists(path) && File.ReadAllLines(path).Length == 1;
    public static string GetDefaultPath() => Path.Combine(PathsBuilder.GetDataDirectory, "Token.txt");

    public bool VerifyToken() {

        string cipherText = GetToken();

        if(string.IsNullOrEmpty(cipherText)) {
            SetToken();
            return VerifyToken();
        }

        try {

            string decryptedText = cryptoService.Decrypt(cipherText);
            return decryptedText == plainText;

        }
        catch(CryptographicException) {
            return false;
        }

    }

    private void SetToken() {
        File.WriteAllText(tokenPath, cryptoService.Encrypt(plainText));
    }

    private string GetToken() {
        if(!File.Exists(tokenPath)) {
            SetToken();
        }

        return File.ReadAllText(tokenPath);
    }

}
