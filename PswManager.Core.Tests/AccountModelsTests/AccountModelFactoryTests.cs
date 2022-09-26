using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
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
		var encryptedModelFromModel = _sut.CreateEncryptedAccount(encryptedValues);

		Assert.True(encryptedModel.IsEncrypted);
		Assert.False(encryptedModel.IsPlainText);
        AccountModelAsserts.AssertEqual(encryptedValues, encryptedModel);
        Assert.IsType<EncryptedAccount>(encryptedModel);

        Assert.True(encryptedModelFromModel.IsEncrypted);
        Assert.False(encryptedModelFromModel.IsPlainText);
        AccountModelAsserts.AssertEqual(encryptedValues, encryptedModelFromModel);
        Assert.IsType<EncryptedAccount>(encryptedModelFromModel);

    }

	[Fact]
	public void CreateDecryptedModel() {

		var values = GetDefault();
		var model = _sut.CreateDecryptedAccount(values.Name, values.Password, values.Email);
		var modelFromModel = _sut.CreateDecryptedAccount(values);

        Assert.False(model.IsEncrypted);
        Assert.True(model.IsPlainText);
        AccountModelAsserts.AssertEqual(values, model);
        Assert.IsType<DecryptedAccount>(model);

        Assert.False(modelFromModel.IsEncrypted);
        Assert.True(modelFromModel.IsPlainText);
        AccountModelAsserts.AssertEqual(values, modelFromModel);
        Assert.IsType<DecryptedAccount>(modelFromModel);

    }

	private static AccountModel GetDefault() => AccountModelMocks.GenerateValidFromName("SomeName");

}