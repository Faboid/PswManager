using PswManagerEncryption.Cryptography;
using PswManagerEncryption.Services;
using PswManagerHelperMethods;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly:InternalsVisibleTo("PswManagerTests")]
namespace PswManagerLibrary.Cryptography {
    internal class Token {

        private readonly ICryptoService cryptoService;
        private readonly string tokenPath;
        private const string plainText = "This is the correct password.";

        public Token(char[] password) : this(new Key(password)) { }

        public Token(char[] password, string customPath) : this(new Key(password), customPath) { }

        public Token(Key key) : this(key, GetDefaultPath()) { }

        public Token(Key key, string customPath) : this(new CryptoService(key), customPath) { }

        internal Token(ICryptoService cryptoService) : this(cryptoService, GetDefaultPath()) { }

        internal Token(ICryptoService cryptoService, string customPath) {
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
}
