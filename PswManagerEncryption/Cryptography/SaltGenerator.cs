using System.Security.Cryptography;

namespace PswManagerEncryption.Cryptography {
    internal class SaltGenerator {

        readonly private int length;

        public SaltGenerator(char[] password) {
            length = password.Sum(x => x / (password.Length / 2));
        }

        public byte[] CreateSalt() => RandomNumberGenerator.GetBytes(length);

        public byte[] ExtractSalt(ref string text) {
            byte[] bytes = Convert.FromBase64String(text);
            byte[] salt = bytes.Take(length).ToArray();
            text = Convert.ToBase64String(bytes.Skip(length).ToArray());
            return salt;
        }

    }
}
