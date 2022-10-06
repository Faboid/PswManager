using PswManager.Core.Validators;
using static PswManager.Core.IAccountFactory;
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
        validatorMock.Setup(x => x.IsAccountValid(It.IsAny<IExtendedAccountModel>())).Returns(validationError);
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
        connectionMock.Setup(x => x.CreateAccountAsync(It.IsAny<IReadOnlyAccountModel>())).Returns(Task.FromResult(errorCode));
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), Mock.Of<IAccountModelFactory>());
        var actual = await sut.CreateAccountAsync(CreateDefaultEncrypted());
        Assert.Equal(expected, actual.OrDefaultError());

    }

    [Fact]
    public async Task CreateAccount_CorrectValues() {

        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.CreateAccountAsync(It.IsAny<IReadOnlyAccountModel>())).Returns(Task.FromResult(CreatorResponseCode.Success));
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), Mock.Of<IAccountModelFactory>());
        var decrypted = CreateDefaultDecrypted();
        var expected = decrypted.GetEncryptedAccount();
        var actual = await sut.CreateAccountAsync(decrypted);
        AccountModelAsserts.AssertEqual(expected, actual.OrDefault());

    }

    [Fact]
    public async Task LoadAccounts_Loads() {

        var models = new Option<IAccountModel, (string name, ReaderErrorCode errorCode)>[] {
            new AccountModel("SomeName", "SomePassword", "SomeEmail"),
            new AccountModel("SomeOtherName", "rwgre", "Email@here.com")
        }.ToAsyncEnumerable();

        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.EnumerateAccountsAsync()).Returns(models);
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), new AccountModelFactory(ICryptoAccountMocks.GetReversedAndSummingCryptor()));

        var result = sut.LoadAccounts();

        var expected = await models.Select(x => x.OrDefault()).Where(x => x != null).OrderBy(x => x.Name).ToArrayAsync();
        var actual = await result.OrderBy(x => x.Name).ToArrayAsync();

        Assert.Equal(expected.Length, actual.Length);

        for(int i = 0; i < actual.Length; i++) {
            AccountModelAsserts.AssertEqual(expected[i], actual[i]);
        }
    }

    public static IEnumerable<object[]> GetCorruptedData() {
        static object[] NewObject(string name, ReaderErrorCode errorCode, string expectedMessage) => new object[] { name, errorCode, expectedMessage };
        yield return NewObject("SomeName", ReaderErrorCode.UsedElsewhere, "SomeName couldn't be loaded because it was used elsewhere.");
        yield return NewObject("Anotherone", ReaderErrorCode.Undefined, "There has been an unknown error trying to load Anotherone");
    }

    [Theory]
    [MemberData(nameof(GetCorruptedData))]
    public async Task LoadAccounts_LoadCorrupted(string name, ReaderErrorCode errorCode, string expectedMessage) {

        var models = new Option<IAccountModel, (string name, ReaderErrorCode errorCode)>[] {
            (name, errorCode),
        }.ToAsyncEnumerable();

        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.EnumerateAccountsAsync()).Returns(models);
        var sut = new AccountFactory(connectionMock.Object, new AccountValidator(), new AccountModelFactory(ICryptoAccountMocks.GetReversedAndSummingCryptor()));

        var result = sut.LoadAccounts();

        var account = await result.SingleAsync();

        Assert.Equal(name, account.Name);
        Assert.Equal(expectedMessage, account.Password);
        Assert.Equal(expectedMessage, account.Email);

    }

    private static EncryptedAccount CreateDefaultEncrypted() => new("SomeName", "SomePassword", "AnEmail@com", ICryptoAccountMocks.GetReversedAndSummingCryptor());
    private static DecryptedAccount CreateDefaultDecrypted() => new("SomeName", "SomePassword", "AnEmail@com", ICryptoAccountMocks.GetReversedAndSummingCryptor());


}