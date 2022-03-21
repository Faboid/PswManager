using PswManagerEncryption.Cryptography;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("PswManagerTests")]
namespace PswManagerEncryption.Services {
    public class CryptoService : ICryptoService, IDisposable {
        //This class is built upon a derived verions of "A Gazhal"'s answer https://stackoverflow.com/a/27484425/16018958

        private readonly Versioning versioning;
        private readonly SaltGenerator saltGenerator;
        private readonly Key key;

        internal CryptoService(char[] password, string version) {
            saltGenerator = new SaltGenerator(password);
            key = new Key(password);
            versioning = new Versioning(version);
        }

        public CryptoService(char[] password) : this(password, "1.00") { }

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

        private Aes GetAes(byte[] salt, string version) {
            Aes aes = Aes.Create();
            var rfc = new Rfc2898DeriveBytes(Encoding.Unicode.GetBytes(key.Get()), salt, Versioning.GetRfcDerivedBytesIterations(version));
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