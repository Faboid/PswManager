using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Mocks;
using PswManager.Core.Validators;
using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Tests.AccountTests;

public class EditAccountTests {

    public EditAccountTests() {
        _cryptoAccountService = ICryptoAccountMocks.GetReversedAndSummingCryptor();
        _accountModelFactory = new AccountModelFactory(_cryptoAccountService);
    }

    private readonly IAccountModelFactory _accountModelFactory;
    private readonly ICryptoAccountService _cryptoAccountService;


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
    [InlineData(EditorErrorCode.Undefined, EditAccountResult.Unknown)]
    [InlineData(EditorErrorCode.NewNameUsedElsewhere, EditAccountResult.NewNameIsOccupied)]
    [InlineData(EditorErrorCode.NewNameExistsAlready, EditAccountResult.NewNameIsOccupied)]
    public async Task EditAccountFailsByExecution(EditorErrorCode errorCode, EditAccountResult expected) {

        var account = _accountModelFactory.CreateEncryptedAccount(GetGeneric());
        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.UpdateAccountAsync(account.Name, It.IsAny<AccountModel>())).Returns(() => ValueTask.FromResult<Option<EditorErrorCode>>(errorCode));

        var sut = new Account(account, connectionMock.Object, new AccountValidator());
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(expected, actual);

    }

    [Fact]
    public async Task EditAccountEncryptsTheNewValues() {

        var decryptedModel = _accountModelFactory.CreateDecryptedAccount(GetGeneric());
        var expected = await decryptedModel.GetEncryptedAccountAsync();
        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.UpdateAccountAsync(expected.Name, It.IsAny<AccountModel>())).Returns(() => ValueTask.FromResult(Option.None<EditorErrorCode>()));

        var sut = new Account(_accountModelFactory.CreateEncryptedAccount(new("AName", "APassword","AnEmail")), connectionMock.Object, new AccountValidator());

        var actual = await sut.EditAccountAsync(decryptedModel);

        Assert.Equal(EditAccountResult.Success, actual);
        Assert.Equal(expected.Name, sut.Name);
        Assert.Equal(expected.Password, sut.EncryptedPassword);
        Assert.Equal(expected.Email, sut.EncryptedEmail);

    }

    private static AccountModel GetGeneric() => new("SomeName", "SomePassword", "SomeEmail");

}