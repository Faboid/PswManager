using PswManager.Core.Services;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Core.AccountModels;

public class DecryptedAccount : IExtendedAccountModel {

	private readonly ICryptoAccountService _cryptoAccountService;

	public DecryptedAccount(string name, string password, string email, ICryptoAccountService cryptoAccountService) {
		_cryptoAccountService = cryptoAccountService;
		Name = name;
		Password = password;
		Email = email;
	}

	public string Name { get; init; }
	public string Password { get; init; }
	public string Email { get; init; }

	public bool IsEncrypted => false;
	public bool IsPlainText => true;

	public DecryptedAccount GetDecryptedAccount() => this;
	public Task<DecryptedAccount> GetDecryptedAccountAsync() => Task.FromResult(this);
	public EncryptedAccount GetEncryptedAccount()
		=> new(Name, _cryptoAccountService.GetPassCryptoService().Encrypt(Password), _cryptoAccountService.GetEmaCryptoService().Encrypt(Email), _cryptoAccountService);
	public Task<EncryptedAccount> GetEncryptedAccountAsync() => Task.Run(GetEncryptedAccount);
	public AccountModel GetUnderlyingModel() => new(Name, Password, Email);
}
