using PswManager.Encryption.Random;
using System.Security.Cryptography;

namespace PswManager.Encryption.Cryptography;

/// <summary>
/// Provides methods to generate and extract salt.
/// </summary>
internal class SaltGenerator {

    readonly private int length;
    readonly private int minLength = 32;
    readonly private int maxLength = 64;

    /// <summary>
    /// Instantiates a <see cref="SaltGenerator"/> with a custom length based on <paramref name="password"/>.
    /// </summary>
    /// <param name="password"></param>
    public SaltGenerator(char[] password) {
        int passValue = password.Sum(x => x / (password.Length / 2));
        length = new SaltRandom(minLength, maxLength, passValue).Next();
    }

    /// <summary>
    /// <inheritdoc cref="RandomNumberGenerator.GetBytes(int)"/>
    /// </summary>
    /// <returns></returns>
    public byte[] CreateSalt() => RandomNumberGenerator.GetBytes(length);

    /// <summary>
    /// Extracts the salt of an already encrypted text. The text will be set to be without those salt bytes.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>The bytes of salt.</returns>
    public byte[] ExtractSalt(ref string text) {
        byte[] bytes = Convert.FromBase64String(text);
        byte[] salt = bytes.Take(length).ToArray();
        text = Convert.ToBase64String(bytes.Skip(length).ToArray());
        return salt;
    }

}
