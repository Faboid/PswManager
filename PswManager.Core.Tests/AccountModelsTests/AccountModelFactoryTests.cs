using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.AccountModelsTests;

public class AccountModelFactoryTests {

	public AccountModelFactoryTests() {
		_cryptoAccountService = ICryptoAccountMocks.GetReversedAndSummingCryptor();
        _sut = new AccountModelFactory(_cryptoAccountService);
	}

	private readonly ICryptoAccountService _cryptoAccountService;
    private readonly IAccountModelFactory _sut;

	[Fact]
	public void CreateEncryptedModel() {

		var encryptedValues = _cryptoAccountService.Encrypt(GetDefault());

		var encryptedModel = _sut.CreateEncryptedAccount(encryptedValues.Name, encryptedValues.Password, encryptedValues.Email);

		Assert.True(encryptedModel.IsEncrypted);
		Assert.False(encryptedModel.IsPlainText);
		Assert.Equal(encryptedValues.Name, encryptedModel.Name);
		Assert.Equal(encryptedValues.Password, encryptedModel.Password);
		Assert.Equal(encryptedValues.Email, encryptedModel.Email);
        Assert.IsType<EncryptedAccount>(encryptedModel);

	}

	[Fact]
	public void CreateDecryptedModel() {

		var values = GetDefault();
		var model = _sut.CreateDecryptedAccount(values.Name, values.Password, values.Email);

        Assert.False(model.IsEncrypted);
        Assert.True(model.IsPlainText);
        Assert.Equal(values.Name, model.Name);
        Assert.Equal(values.Password, model.Password);
        Assert.Equal(values.Email, model.Email);
        Assert.IsType<DecryptedAccount>(model);

    }

	private static AccountModel GetDefault() => AccountModelMocks.GenerateValidFromName("SomeName");

}