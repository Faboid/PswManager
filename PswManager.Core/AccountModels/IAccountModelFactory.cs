namespace PswManager.Core.AccountModels;

/// <summary>
/// Provides methods to instance <see cref="IAccountModel"/>.
/// </summary>
public interface IAccountModelFactory {

    /// <summary>
    /// Takes in encrypted values to create an encrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    IAccountModel CreateDecryptedAccount(string name, string password, string email);

    /// <summary>
    /// Takes in plain-text values to create a decrypted account model.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    IAccountModel CreateEncryptedAccount(string name, string password, string email);
}