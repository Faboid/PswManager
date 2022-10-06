using System.Security.Cryptography;

namespace PswManager.Encryption.Services;

/// <summary>
/// Provides methods to encrypt/decrypt a <see cref="string"/>.
/// </summary>
public interface ICryptoService {

    /// <summary>
    /// Encrypts <paramref name="plainText"/> and returns its encrypted version.
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts <paramref name="cipherText"/> and returns its plain-text version.
    /// </summary>
    /// <remarks>
    /// If the password isn't the same as the one used to encrypt <paramref name="cipherText"/>, it will throw a <see cref="CryptographicException"/>
    /// </remarks>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    /// <exception cref="CryptographicException"></exception>
    string Decrypt(string cipherText);

}
