using PswManager.Core.Services;
namespace PswManager.Core.AccountModels;

public class AccountModelFactory : IAccountModelFactory {

	private readonly ICryptoAccountService _cryptoAccountService;

	public AccountModelFactory(ICryptoAccountService cryptoAccountService) {
		_cryptoAccountService = cryptoAccountService;
	}

	/// <summary>
	/// Takes in encrypted values to create an encrypted account model.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="password"></param>
	/// <param name="email"></param>
	/// <returns></returns>
	public IAccountModel CreateEncryptedAccount(string name, string password, string email) {
		return new EncryptedAccount(name, password, email, _cryptoAccountService);
	}

	/// <summary>
	/// Takes in plain-text values to create a decrypted account model.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="password"></param>
	/// <param name="email"></param>
	/// <returns></returns>
	public IAccountModel CreateDecryptedAccount(string name, string password, string email) {
		return new DecryptedAccount(name, password, email, _cryptoAccountService);
	}

}
