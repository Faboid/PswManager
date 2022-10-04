using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Tests.Mocks;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;

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

        var sut = CreateAccount(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), Mock.Of<IDataConnection>(), Mock.Of<IAccountValidator>());
        await sut.DeleteAccountAsync();
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(EditAccountResult.DoesNotExist, actual);

    }

    [Fact]
    public async Task DeleteAccountCallsConnection() {

        var connectionMock = new Mock<IDataConnection>();
        var sut = CreateAccount(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), connectionMock.Object, Mock.Of<IAccountValidator>());
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
        validatorMock.Setup(x => x.IsAccountValid(It.IsAny<IExtendedAccountModel>())).Returns(accountValid);
        var connectionMock = new Mock<IDataConnection>();

        var sut = CreateAccount(_accountModelFactory.CreateEncryptedAccount(GetGeneric()), Mock.Of<IDataConnection>(), validatorMock.Object);
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
        connectionMock.Setup(x => x.UpdateAccountAsync(account.Name, It.IsAny<IReadOnlyAccountModel>())).Returns(Task.FromResult(errorCode));

        var sut = CreateAccount(account, connectionMock.Object, new AccountValidator());
        var actual = await sut.EditAccountAsync(_accountModelFactory.CreateEncryptedAccount(GetGeneric()));
        Assert.Equal(expected, actual);

    }

    [Fact]
    public async Task EditAccountEncryptsTheNewValues() {

        var decryptedModel = _accountModelFactory.CreateDecryptedAccount(GetGeneric());
        var expected = await decryptedModel.GetEncryptedAccountAsync();
        var connectionMock = new Mock<IDataConnection>();
        connectionMock.Setup(x => x.UpdateAccountAsync("AName", It.IsAny<IReadOnlyAccountModel>())).Returns(Task.FromResult(EditorResponseCode.Success));

        var sut = CreateAccount(_accountModelFactory.CreateEncryptedAccount(new AccountModel("AName", "APassword", "AnEmail")), connectionMock.Object, new AccountValidator());

        var actual = await sut.EditAccountAsync(decryptedModel);

        Assert.Equal(EditAccountResult.Success, actual);
        Assert.Equal(expected.Name, sut.Name);
        Assert.Equal(expected.Password, sut.Password);
        Assert.Equal(expected.Email, sut.Email);

    }

    [Fact]
    public async Task CorruptedAccount_DoesNotEdit() {

        var holder = new CorruptedAccountHolder("AName", ReaderErrorCode.Undefined, _accountModelFactory);
        var account = new Account(holder, Mock.Of<IDataConnection>());
        var editingValues = _accountModelFactory.CreateDecryptedAccount("name", "pass", "ema");
        var result = await account.EditAccountAsync(editingValues);
        var holderResult = await holder.EditAccountAsync(editingValues);

        Assert.Equal(EditAccountResult.ValuesCorrupted, result);
        Assert.Equal(EditAccountResult.ValuesCorrupted, holderResult);

    }

    private static AccountModel GetGeneric() => new("SomeName", "SomePassword", "SomeEmail");

    private static IAccount CreateAccount(EncryptedAccount encryptedAccount, IDataConnection _connection, IAccountValidator _validator) {
        var accountHolder = new AccountHolder(encryptedAccount, _validator, _connection);
        return new Account(accountHolder, _connection);
    }

}