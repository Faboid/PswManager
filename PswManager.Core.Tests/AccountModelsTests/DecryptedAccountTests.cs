using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
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
        AccountModelAsserts.AssertEqual(expected, actual);

        Assert.False(actualAsync.IsEncrypted);
        Assert.True(actualAsync.IsPlainText);
        Assert.Equal(expected, actualAsync);
        AccountModelAsserts.AssertEqual(expected, actualAsync);

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
        AccountModelAsserts.AssertEqual(expectedValues, encryptedAccount);

        Assert.True(encryptedAccountAsync.IsEncrypted);
        Assert.False(encryptedAccountAsync.IsPlainText);
        AccountModelAsserts.AssertEqual(expectedValues, encryptedAccountAsync);

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
