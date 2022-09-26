using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;

namespace PswManager.Core.Tests.AccountModelsTests;

public class EncryptedAccountTests {

	private readonly ICryptoAccountService _cryptoAccountService = ICryptoAccountMocks.GetReversedAndSummingCryptor();

	private EncryptedAccount GetDefault() => new("SomeName", "SomePass", "SomeEma", _cryptoAccountService);

	[Fact]
	public void GetUnderlyingModel() {

		var expected = GetDefault();
		var actual = expected.GetUnderlyingModel();
		AccountModelAsserts.AssertEqual(expected, actual);

	}

    [Fact]
	public async Task GetEncryptedModel_ReturnsItself() {

		var expected = GetDefault();
		var actual = expected.GetEncryptedAccount();
		var actualAsync = await expected.GetEncryptedAccountAsync();

        Assert.True(actual.IsEncrypted);
        Assert.False(actual.IsPlainText);
        Assert.Equal(expected, actual);
		AccountModelAsserts.AssertEqual(expected, actual);

        Assert.True(actualAsync.IsEncrypted);
        Assert.False(actualAsync.IsPlainText);
        Assert.Equal(expected, actualAsync);
		AccountModelAsserts.AssertEqual(expected, actualAsync);

    }

	[Fact]
	public async Task GetDecryptedModel_DecryptsValues() {

		var expectedValues = AccountModelMocks.GenerateValidFromName("SomeName");
		var encryptedValues = _cryptoAccountService.Encrypt(expectedValues);
		var encryptedAccount = new EncryptedAccount(encryptedValues.Name, encryptedValues.Password, encryptedValues.Email, _cryptoAccountService);

		var decryptedAccount = encryptedAccount.GetDecryptedAccount();
		var decryptedAccountAsync = await encryptedAccount.GetDecryptedAccountAsync();

		Assert.False(decryptedAccount.IsEncrypted);
		Assert.True(decryptedAccount.IsPlainText);
		Assert.Equal(expectedValues.Name, decryptedAccount.Name);
		Assert.Equal(expectedValues.Password, decryptedAccount.Password);
		Assert.Equal(expectedValues.Email, decryptedAccount.Email);

        Assert.False(decryptedAccountAsync.IsEncrypted);
        Assert.True(decryptedAccountAsync.IsPlainText);
        Assert.Equal(expectedValues.Name, decryptedAccountAsync.Name);
        Assert.Equal(expectedValues.Password, decryptedAccountAsync.Password);
        Assert.Equal(expectedValues.Email, decryptedAccountAsync.Email);

    }

	[Fact]
	public void IsEncrypted_ShouldBe_True() {
		Assert.True(GetDefault().IsEncrypted);
	}

    [Fact]
    public void IsPlainText_ShouldBe_False() {
        Assert.False(GetDefault().IsPlainText);
    }


}