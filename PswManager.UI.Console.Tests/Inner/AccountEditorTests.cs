using Moq;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.UI.Console.Inner;

namespace PswManager.UI.Console.Tests.Inner;
public class AccountEditorTests {

    public AccountEditorTests() {
        cryptoAccount = new CryptoAccountService(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

        dataEditorMock = new Mock<IDataEditor>();
        dataEditorMock
            .Setup(x => x.UpdateAccountAsync(It.IsAny<string>(), It.IsAny<AccountModel>()))
            .Returns<string, AccountModel>((x, y) => (string.IsNullOrWhiteSpace(x) ? EditorResponseCode.InvalidName : EditorResponseCode.Success).AsTask());
    }

    readonly Mock<IDataEditor> dataEditorMock;
    readonly ICryptoAccountService cryptoAccount;

    [Theory]
    [InlineData("accName", "noEncrypt", "password", "email")]
    [InlineData("accName", "   ", null, "email")]
    public async Task UpdateValuesGotEncryptedAsync(string name, string newName, string password, string email) {

        //arrange
        var (editor, input, expected) = ArrangeTest(newName, password, email);

        //act
        var result = await editor.UpdateAccountAsync(name, input);

        //assert
        Assert.Equal(EditorResponseCode.Success, result);
        dataEditorMock.Verify(x => x.UpdateAccountAsync(
                It.Is<string>(x => x == name),
                It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))
            ));
        dataEditorMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task NoCallsIfInvalidName(string name) {

        //arrange
        var input = new AccountModel("newName", "some", "newEma");
        AccountEditor editor = new(dataEditorMock.Object, cryptoAccount);

        //act
        var result = editor.UpdateAccount(name, input);
        var resultAsync = await editor.UpdateAccountAsync(name, input);

        //assert
        Assert.Equal(EditorResponseCode.InvalidName, result);
        Assert.Equal(EditorResponseCode.InvalidName, resultAsync);
        dataEditorMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task MethodCallsArePure() {

        //arrange
        var expected = new AccountModel("SomeName", "SomePassword", "SomeEmail");
        var actual = new AccountModel(expected.Name, expected.Password, expected.Email);
        var sut = new AccountEditor(dataEditorMock.Object, cryptoAccount);

        //act
        _ = sut.UpdateAccount(actual.Name, actual);
        _ = await sut.UpdateAccountAsync(actual.Name, actual);

        //assert
        AccountModelAsserts.AssertEqual(expected, actual);

    }

    private (AccountEditor editor, IAccountModel input, IAccountModel expected) ArrangeTest(string name, string password, string email) {
        var editor = new AccountEditor(dataEditorMock.Object, cryptoAccount);
        var input = new AccountModel(name, password, email);
        var expected = cryptoAccount.Encrypt(input);
        return (editor, input, expected);
    }

}
