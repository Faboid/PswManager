using PswManager.Core.Services;
using System.Threading.Tasks;

namespace PswManager.Core.AccountModels;

public class EncryptedAccount : IAccountModel {

	private readonly ICryptoAccountService _cryptoAccountService;

	public EncryptedAccount(string name, string password, string email, ICryptoAccountService cryptoAccountService) {
		_cryptoAccountService = cryptoAccountService;
		Name = name;
		Password = password;
		Email = email;
	}

	/// <summary>
	/// The name of the account. Names are kept plain-text.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// The encrypted password of the account.
	/// </summary>
	public string Password { get; private set; }

	/// <summary>
	/// The encrypted email of the account.
	/// </summary>
	public string Email { get; private set; }

	public bool IsEncrypted => true;
	public bool IsPlainText => false;

	public DecryptedAccount GetDecryptedAccount()
		=> new(Name, _cryptoAccountService.GetPassCryptoService().Decrypt(Password), _cryptoAccountService.GetEmaCryptoService().Decrypt(Email), _cryptoAccountService);
	public Task<DecryptedAccount> GetDecryptedAccountAsync() => Task.Run(GetDecryptedAccount);
	public EncryptedAccount GetEncryptedAccount() => this;
	public Task<EncryptedAccount> GetEncryptedAccountAsync() => Task.FromResult(this);
}
