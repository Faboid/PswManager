using PswManager.Core.Validators;
using static PswManager.Core.AccountFactory;
using PswManager.Core.AccountModels;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using PswManager.Core.Tests.Asserts;
using PswManager.Database;

namespace PswManager.Core.Tests;

public class AccountFactoryTests {

    [Theory]
    [InlineData(AccountValid.Unknown, CreateAccountErrorCode.Unknown)]
    [InlineData(AccountValid.NameEmptyOrNull, CreateAccountErrorCode.NameEmptyOrNull)]
    [InlineData(AccountValid.PasswordEmptyOrNull, CreateAccountErrorCode.PasswordEmptyOrNull)]
    [InlineData(AccountValid.EmailEmptyOrNull, CreateAccountErrorCode.EmailEmptyOrNull)]
    public async Task CreateAccount_FailsValidation(AccountValid validationError, CreateAccountErrorCode expected) {

        var validatorMock = new Mock<IAccountValidator>();
        validatorMock.Setup(x => x.IsAccountValid(It.IsAny<IAccountModel>())).Returns(validationError);
        var sut = new AccountFactory(Mock.Of<IDataConnection>(), validatorMock.Object, Mock.Of<IAccountModelFactory>());

        var actual = await sut.CreateAccountAsync(CreateDefaultEncrypted());
        Assert.Equal(expected, actual.OrDefaultError());

    }

    [Theory]
    [InlineData(CreatorResponseCode.Undefined, CreateAccountErrorCode.Unknown)]
    [InlineData(CreatorResponseCode.InvalidName, CreateAccountErrorCode.NameEmptyOrNull)]
    [InlineData(CreatorResponseCode.MissingPassword, CreateAccountErrorCode.PasswordEmptyOrNull)]
    [InlineData(CreatorResponseCode.MissingEmail, CreateAccountErrorCode.EmailEmptyOrNull)]
    [InlineData(CreatorResponseCode.AccountExistsAlready, CreateAccountErrorCode.NameIsOccupied)]
    [InlineData(CreatorResponseCode.UsedElsewhere, CreateAccountErrorCode.NameIsOccupied)]
    public async Task CreateAccount_FailsDatabaseSaving(CreatorResponseCode errorCode, CreateAccountErrorCode expected) {

        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>())).Returns(Task.FromResult(errorCode));
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), Mock.Of<IAccountModelFactory>());
        var actual = await sut.CreateAccountAsync(CreateDefaultEncrypted());
        Assert.Equal(expected, actual.OrDefaultError());

    }

    [Fact]
    public async Task CreateAccount_CorrectValues() {

        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>())).Returns(Task.FromResult(CreatorResponseCode.Success));
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), Mock.Of<IAccountModelFactory>());
        var decrypted = CreateDefaultDecrypted();
        var expected = decrypted.GetEncryptedAccount();
        var actual = await sut.CreateAccountAsync(decrypted);
        AccountModelAsserts.AssertEqual(expected, actual.OrDefault());

    }

    [Fact(Skip = "LoadAccounts() might soon receive a change(from the DB), so this test is currently not implemented.")]
    public Task LoadAccounts_Loads() {
        throw new NotImplementedException();
    }

    private static EncryptedAccount CreateDefaultEncrypted() => new("SomeName", "SomePassword", "AnEmail@com", ICryptoAccountMocks.GetReversedAndSummingCryptor());
    private static DecryptedAccount CreateDefaultDecrypted() => new("SomeName", "SomePassword", "AnEmail@com", ICryptoAccountMocks.GetReversedAndSummingCryptor());


}