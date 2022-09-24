using PswManager.Core.Services;
namespace PswManager.Core.AccountModels;

public class AccountModelFactory : IAccountModelFactory {

	private readonly ICryptoAccountService _cryptoAccountService;

	public AccountModelFactory(ICryptoAccountService cryptoAccountService) {
		_cryptoAccountService = cryptoAccountService;
	}

	public IAccountModel CreateEncryptedAccount(string name, string password, string email) {
		return new EncryptedAccount(name, password, email, _cryptoAccountService);
	}

	public IAccountModel CreateDecryptedAccount(string name, string password, string email) {
		return new DecryptedAccount(name, password, email, _cryptoAccountService);
	}

}
