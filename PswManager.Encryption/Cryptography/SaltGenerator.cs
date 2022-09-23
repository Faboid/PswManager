using PswManager.Encryption.Random;
using System.Security.Cryptography;

namespace PswManager.Encryption.Cryptography; 
internal class SaltGenerator {

    readonly private int length;
    readonly private int minLength = 32;
    readonly private int maxLength = 64;

    public SaltGenerator(char[] password) {
        int passValue = password.Sum(x => x / (password.Length / 2));
        length = new SaltRandom(minLength, maxLength, passValue).Next();
    }

    public byte[] CreateSalt() => RandomNumberGenerator.GetBytes(length);

    public byte[] ExtractSalt(ref string text) {
        byte[] bytes = Convert.FromBase64String(text);
        byte[] salt = bytes.Take(length).ToArray();
        text = Convert.ToBase64String(bytes.Skip(length).ToArray());
        return salt;
    }

}
