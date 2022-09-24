using PswManager.Database.Models;

namespace PswManager.Core.AccountModels;

/// <summary>
/// Provides methods to instance <see cref="IAccountModel"/>.
/// </summary>
public interface IAccountModelFactory {

    /// <summary>
    /// Takes in plain-text values to create an decrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    DecryptedAccount CreateDecryptedAccount(string name, string password, string email);

    /// <summary>
    /// Takes in encrypted values to create an encrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    EncryptedAccount CreateEncryptedAccount(string name, string password, string email);


    /// <summary>
    /// Takes in plain-text values to create an decrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    DecryptedAccount CreateDecryptedAccount(AccountModel model);

    /// <summary>
    /// Takes in encrypted values to create an encrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    EncryptedAccount CreateEncryptedAccount(AccountModel model);

}