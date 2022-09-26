using Moq;
using PswManager.ConsoleUI.Inner;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.TestUtils;
using PswManager.Utils;
using PswManager.Utils.Options;

namespace PswManager.ConsoleUI.Tests.Inner;
public class AccountEditorTests {

    public AccountEditorTests() {
        cryptoAccount = new CryptoAccountService(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

        dataEditorMock = new Mock<IDataEditor>();
        dataEditorMock
            .Setup(x => x.UpdateAccountAsync(It.IsAny<string>(), It.IsAny<AccountModel>()))
            .Returns<string, AccountModel>((x, y) => (string.IsNullOrWhiteSpace(x) ? EditorErrorCode.InvalidName : Option.None<EditorErrorCode>()).AsTask());
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
        var option = await editor.UpdateAccountAsync(name, input);

        //assert
        option.Is(OptionResult.None);
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
        var option = editor.UpdateAccount(name, input);
        var optionAsync = await editor.UpdateAccountAsync(name, input);

        //assert
        option.Is(OptionResult.Some);
        optionAsync.Is(OptionResult.Some);
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

    private (AccountEditor editor, AccountModel input, AccountModel expected) ArrangeTest(string name, string password, string email) {
        var editor = new AccountEditor(dataEditorMock.Object, cryptoAccount);
        var input = new AccountModel(name, password, email);
        var expected = cryptoAccount.Encrypt(input);
        return (editor, input, expected);
    }

}
