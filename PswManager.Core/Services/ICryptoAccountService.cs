using PswManager.Database.Models;
using PswManager.Encryption.Services;
using System.Diagnostics.Contracts;

namespace PswManager.Core.Services;
public interface ICryptoAccountService {

    public ICryptoService GetPassCryptoService();
    public ICryptoService GetEmaCryptoService();

    [Pure]
    public (string encryptedPassword, string encryptedEmail) Encrypt(string password, string email);

    [Pure]
    public (string decryptedPassword, string decryptedEmail) Decrypt(string encryptedPassword, string encryptedEmail);

    [Pure]
    public (string encryptedPassword, string encryptedEmail) Encrypt((string password, string email) values);

    [Pure]
    public (string decryptedPassword, string decryptedEmail) Decrypt((string encryptedPassword, string encryptedEmail) values);

    [Pure]
    public IAccountModel Encrypt(IReadOnlyAccountModel model);

    [Pure]
    public IAccountModel Decrypt(IReadOnlyAccountModel model);

}
