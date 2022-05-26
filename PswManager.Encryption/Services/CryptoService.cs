using PswManager.Encryption.Cryptography;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("PswManager.Tests")]
[assembly: InternalsVisibleTo("PswManager.Core.Tests")]
namespace PswManager.Encryption.Services {
    public class CryptoService : ICryptoService, IDisposable {
        //This class is built upon a derived version of "A Gazhal"'s answer https://stackoverflow.com/a/27484425/16018958

        private const string currentVersion = "1.00";
        private readonly Versioning versioning;
        private readonly SaltGenerator saltGenerator;
        private readonly Key key;

        public CryptoService(char[] password) : this(password, currentVersion) { }
        internal CryptoService(char[] password, string version) : this (new Key(password), version){ }
        public CryptoService(Key key) : this(key, currentVersion) { } 

        internal CryptoService(Key key, string version) { //todo - consider whether to clear the given key
            this.key = key;
            saltGenerator = new SaltGenerator(key.Get());
            versioning = new Versioning(version);
        }

        public string Encrypt(string plainText) {

            byte[] bytes = Encoding.Unicode.GetBytes(plainText);
            byte[] salt = saltGenerator.CreateSalt();

            using Aes encryptor = GetAes(salt, versioning.GetVersion());
            using var ms = new MemoryStream();
            WriteToStream(ms, encryptor.CreateEncryptor(), bytes);

            byte[] output = salt.Concat(ms.ToArray()).ToArray();
            output = versioning.AddVersion(output);
            return Convert.ToBase64String(output);
        }

        public string Decrypt(string cipherText) {

            string version = Versioning.ExtractVersion(ref cipherText);
            byte[] salt = saltGenerator.ExtractSalt(ref cipherText);
            cipherText = cipherText.Replace(" ", "+");
            byte[] bytes = Convert.FromBase64String(cipherText);

            using Aes encryptor = GetAes(salt, version);
            using var ms = new MemoryStream();
            WriteToStream(ms, encryptor.CreateDecryptor(), bytes);
            return Encoding.Unicode.GetString(ms.ToArray());
        }

        private Aes GetAes(byte[] salt, string version) { //todo - this is very slow. Attempt optimizing
            Aes aes = Aes.Create();
            using var rfc = new Rfc2898DeriveBytes(Encoding.Unicode.GetBytes(key.Get()), salt, Versioning.GetRfcDerivedBytesIterations(version));
            aes.Key = rfc.GetBytes(32);
            aes.IV = rfc.GetBytes(16);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            return aes;
        }

        private static void WriteToStream(MemoryStream ms, ICryptoTransform transform, byte[] bytes) {
            using CryptoStream cs = new(ms, transform, CryptoStreamMode.Write);
            cs.Write(bytes);
        }

        public void Dispose() {
            key.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}