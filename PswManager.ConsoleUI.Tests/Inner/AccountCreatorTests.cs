using Moq;
using PswManager.ConsoleUI.Inner;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.TestUtils;
using PswManager.Utils.Options;

namespace PswManager.ConsoleUI.Tests.Inner;
public class AccountCreatorTests {

    public AccountCreatorTests() {
        cryptoAccount = new CryptoAccountService(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

        dataCreatorMock = new Mock<IDataCreator>();
        dataCreatorMock
            .Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>()))
            .Returns<AccountModel>(x => Task.FromResult(OptionMocks.ValidateValues(x)));
    }

    //since the purpose of this class is encrypting the password & email,
    //and then passing the values down to the database, there's no need to test the database itself.
    readonly Mock<IDataCreator> dataCreatorMock;

    //a single version to produce consistent results
    readonly ICryptoAccountService cryptoAccount;

    [Theory]
    [InlineData("asyncName", "newpass", "ema@here.com")]
    public void AccountObjectGotEncrypted(string name, string password, string email) {

        //arrange
        var (creator, input, expected) = ArrangeTest(name, password, email);

        //act
        var result = creator.CreateAccount(input);

        //assert
        result.Is(OptionResult.None);
        dataCreatorMock.Verify(x => x.CreateAccountAsync(It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))));
        dataCreatorMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData("asyncName", "newpass", "ema@here.com")]
    public async Task AccountObjectGotEncryptedAsync(string name, string password, string email) {

        //arrange
        var (creator, input, expected) = ArrangeTest(name, password, email);

        //act
        var result = await creator.CreateAccountAsync(input);

        //assert
        result.Is(OptionResult.None);
        dataCreatorMock.Verify(x => x.CreateAccountAsync(It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))));
        dataCreatorMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task NoCallsIfInvalidName(string name) {

        //arrange
        var input = new AccountModel(name, "some", "newEma");
        AccountCreator creator = new(dataCreatorMock.Object, cryptoAccount);

        //act
        var actual = creator.CreateAccount(input);
        var actualAsync = await creator.CreateAccountAsync(input);

        //assert
        actual.Is(OptionResult.Some);
        actualAsync.Is(OptionResult.Some);
        dataCreatorMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task MethodCallsArePure() {

        //arrange
        var expected = new AccountModel("SomeName", "SomePassword", "SomeEmail");
        var actual = new AccountModel(expected.Name, expected.Password, expected.Email);
        var sut = new AccountCreator(dataCreatorMock.Object, cryptoAccount);

        //act
        _ = await sut.CreateAccountAsync(actual);

        //assert
        AccountModelAsserts.AssertEqual(expected, actual);

    }

    private (AccountCreator creator, AccountModel input, AccountModel expected) ArrangeTest(string name, string password, string email) {
        var creator = new AccountCreator(dataCreatorMock.Object, cryptoAccount);
        var input = new AccountModel(name, password, email);
        var expected = cryptoAccount.Encrypt(input);
        return (creator, input, expected);
    }

}
