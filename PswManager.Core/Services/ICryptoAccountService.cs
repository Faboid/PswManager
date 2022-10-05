using PswManager.Database.Models;
using PswManager.Encryption.Services;
using System.Diagnostics.Contracts;

namespace PswManager.Core.Services;

/// <summary>
/// Provides methods to encrypt and decrypt an account.
/// </summary>
public interface ICryptoAccountService {

    /// <summary>
    /// Gets the <see cref="ICryptoService"/> used to encrypt passwords.
    /// </summary>
    /// <returns></returns>
    public ICryptoService GetPassCryptoService();

    /// <summary>
    /// Gets the <see cref="ICryptoService"/> used to encrypt emails.
    /// </summary>
    /// <returns></returns>
    public ICryptoService GetEmaCryptoService();

    /// <summary>
    /// Encrypts the two given strings, each with their corresponding <see cref="ICryptoService"/>.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    [Pure]
    public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

    /// <summary>
    /// Decrypts the two given strings, each with their corresponding <see cref="ICryptoService"/>.
    /// </summary>
    /// <param name="encryptedPassword"></param>
    /// <param name="encryptedEmail"></param>
    /// <returns></returns>
    [Pure]
    public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);

    /// <summary>
    /// Encrypts the two given strings, each with their corresponding <see cref="ICryptoService"/>.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [Pure]
    public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

    /// <summary>
    /// Decrypts the two given strings, each with their corresponding <see cref="ICryptoService"/>.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [Pure]
    public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

    /// <summary>
    /// Encrypts the account's password and email.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Pure]
    public IAccountModel Encrypt(IReadOnlyAccountModel model);

    /// <summary>
    /// Decrypts the account's password and email.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Pure]
    public IAccountModel Decrypt(IReadOnlyAccountModel model);

}
