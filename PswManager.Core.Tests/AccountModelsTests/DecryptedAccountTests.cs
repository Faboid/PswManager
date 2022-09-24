using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Mocks;

namespace PswManager.Core.Tests.AccountModelsTests;

public class DecryptedAccountTests {

    private readonly ICryptoAccountService _cryptoAccountService = ICryptoAccountMocks.GetReversedAndSummingCryptor();

    private DecryptedAccount GetDefault() => new("SomeName", "SomePass", "SomeEma", _cryptoAccountService);

    [Fact]
    public async Task GetDecryptedModel_ReturnsItself() {

        var expected = GetDefault();
        var actual = expected.GetDecryptedAccount();
        var actualAsync = await expected.GetDecryptedAccountAsync();

        Assert.False(actual.IsEncrypted);
        Assert.True(actual.IsPlainText);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);

        Assert.False(actualAsync.IsEncrypted);
        Assert.True(actualAsync.IsPlainText);
        Assert.Equal(expected, actualAsync);
        Assert.Equal(expected.Name, actualAsync.Name);
        Assert.Equal(expected.Password, actualAsync.Password);
        Assert.Equal(expected.Email, actualAsync.Email);

    }

    [Fact]
    public async Task GetEncryptedModel_EncryptsValues() {

        var values = AccountModelMocks.GenerateValidFromName("SomeName");
        var expectedValues = _cryptoAccountService.Encrypt(values);
        var decryptedAccount = new DecryptedAccount(values.Name, values.Password, values.Email, _cryptoAccountService);

        var encryptedAccount = decryptedAccount.GetEncryptedAccount();
        var encryptedAccountAsync = await decryptedAccount.GetEncryptedAccountAsync();

        Assert.True(encryptedAccount.IsEncrypted);
        Assert.False(encryptedAccount.IsPlainText);
        Assert.Equal(expectedValues.Name, encryptedAccount.Name);
        Assert.Equal(expectedValues.Password, encryptedAccount.Password);
        Assert.Equal(expectedValues.Email, encryptedAccount.Email);

        Assert.True(encryptedAccountAsync.IsEncrypted);
        Assert.False(encryptedAccountAsync.IsPlainText);
        Assert.Equal(expectedValues.Name, encryptedAccountAsync.Name);
        Assert.Equal(expectedValues.Password, encryptedAccountAsync.Password);
        Assert.Equal(expectedValues.Email, encryptedAccountAsync.Email);

    }

    [Fact]
    public void IsEncrypted_ShouldBe_False() {
        Assert.False(GetDefault().IsEncrypted);
    }

    [Fact]
    public void IsPlainText_ShouldBe_True() {
        Assert.True(GetDefault().IsPlainText);
    }


}
