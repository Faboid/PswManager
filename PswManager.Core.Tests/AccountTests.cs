using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Mocks;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Tests;

public class AccountTests {

    public AccountTests() {
        _cryptoAccountService = ICryptoAccountMocks.GetReversedAndSummingCryptor();
        _accountModelFactory = new AccountModelFactory(_cryptoAccountService);
    }

    private readonly IAccountModelFactory _accountModelFactory;
    private readonly ICryptoAccountService _cryptoAccountService;

    [Fact]
    public async Task EditingAfterDeletingReturns_DoesNotExist() {

        var sut = new Account(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), Mock.Of<IDataConnection>(), Mock.Of<IAccountValidator>());
        await sut.DeleteAccountAsync();
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(EditAccountResult.DoesNotExist, actual);

    }

    [Fact]
    public async Task DeleteAccountCallsConnection() {

        var connectionMock = new Mock<IDataConnection>();
        var sut = new Account(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), connectionMock.Object, Mock.Of<IAccountValidator>());
        await sut.DeleteAccountAsync();
        connectionMock.Verify(x => x.DeleteAccountAsync(sut.Name));

    }

    [Theory]
    [InlineData(AccountValid.Unknown, EditAccountResult.Unknown)]
    [InlineData(AccountValid.NameEmptyOrNull, EditAccountResult.NameCannotBeEmpty)]
    [InlineData(AccountValid.PasswordEmptyOrNull, EditAccountResult.PasswordCannotBeEmpty)]
    [InlineData(AccountValid.EmailEmptyOrNull, EditAccountResult.EmailCannotBeEmpty)]
    public async Task EditAccountFailsByValidation(AccountValid accountValid, EditAccountResult expected) {

        var validatorMock = new Mock<IAccountValidator>();
        validatorMock.Setup(x => x.IsAccountValid(It.IsAny<IAccountModel>())).Returns(accountValid);
        var connectionMock = new Mock<IDataConnection>();

        var sut = new Account(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), Mock.Of<IDataConnection>(), validatorMock.Object);
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(expected, actual);

    }

    [Theory]
    [InlineData(EditorResponseCode.Undefined, EditAccountResult.Unknown)]
    [InlineData(EditorResponseCode.NewNameUsedElsewhere, EditAccountResult.NewNameIsOccupied)]
    [InlineData(EditorResponseCode.NewNameExistsAlready, EditAccountResult.NewNameIsOccupied)]
    public async Task EditAccountFailsByExecution(EditorResponseCode errorCode, EditAccountResult expected) {

        var account = _accountModelFactory.CreateEncryptedAccount(GetGeneric());
        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.UpdateAccountAsync(account.Name, It.IsAny<AccountModel>())).Returns(Task.FromResult(errorCode));

        var sut = new Account(account, connectionMock.Object, new AccountValidator());
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(expected, actual);

    }

    [Fact]
    public async Task EditAccountEncryptsTheNewValues() {

        var decryptedModel = _accountModelFactory.CreateDecryptedAccount(GetGeneric());
        var expected = await decryptedModel.GetEncryptedAccountAsync();
        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.UpdateAccountAsync("AName", It.IsAny<AccountModel>())).Returns(Task.FromResult(EditorResponseCode.Success));

        var sut = new Account(_accountModelFactory.CreateEncryptedAccount(new("AName", "APassword", "AnEmail")), connectionMock.Object, new AccountValidator());

        var actual = await sut.EditAccountAsync(decryptedModel);

        Assert.Equal(EditAccountResult.Success, actual);
        Assert.Equal(expected.Name, sut.Name);
        Assert.Equal(expected.Password, sut.EncryptedPassword);
        Assert.Equal(expected.Email, sut.EncryptedEmail);

    }

    private static AccountModel GetGeneric() => new("SomeName", "SomePassword", "SomeEmail");

}