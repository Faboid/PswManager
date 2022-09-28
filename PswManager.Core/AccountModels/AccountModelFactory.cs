using PswManager.Core.Services;
using PswManager.Database.Models;

namespace PswManager.Core.AccountModels;

public class AccountModelFactory : IAccountModelFactory {

    private readonly ICryptoAccountService _cryptoAccountService;

    public AccountModelFactory(ICryptoAccountService cryptoAccountService) {
        _cryptoAccountService = cryptoAccountService;
    }

    public EncryptedAccount CreateEncryptedAccount(string name, string password, string email) {
        return new EncryptedAccount(name, password, email, _cryptoAccountService);
    }

    public DecryptedAccount CreateDecryptedAccount(string name, string password, string email) {
        return new DecryptedAccount(name, password, email, _cryptoAccountService);
    }

    public DecryptedAccount CreateDecryptedAccount(AccountModel model) => CreateDecryptedAccount(model.Name, model.Password, model.Email);
    public EncryptedAccount CreateEncryptedAccount(AccountModel model) => CreateEncryptedAccount(model.Name, model.Password, model.Email);
}
