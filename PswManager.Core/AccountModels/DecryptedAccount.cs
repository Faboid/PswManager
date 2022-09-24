using PswManager.Core.Services;
using System.Threading.Tasks;

namespace PswManager.Core.AccountModels;

public class DecryptedAccount : IAccountModel {

	private readonly ICryptoAccountService _cryptoAccountService;

	public DecryptedAccount(string name, string password, string email, ICryptoAccountService cryptoAccountService) {
		_cryptoAccountService = cryptoAccountService;
		Name = name;
		Password = password;
		Email = email;
	}

	public string Name { get; private set; }
	public string Password { get; private set; }
	public string Email { get; private set; }

	public bool IsEncypted => false;
	public bool IsPlainText => true;

	public DecryptedAccount GetDecryptedAccount() => this;
	public Task<DecryptedAccount> GetDecryptedAccountAsync() => Task.FromResult(this);
	public EncryptedAccount GetEncryptedAccount()
		=> new(Name, _cryptoAccountService.GetPassCryptoService().Encrypt(Password), _cryptoAccountService.GetEmaCryptoService().Encrypt(Email), _cryptoAccountService);
	public Task<EncryptedAccount> GetEncryptedAccountAsync() => Task.Run(GetEncryptedAccount);
}
