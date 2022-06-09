using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.TestUtils;
using PswManager.Utils;
using PswManager.Utils.Options;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountEditorTests {

        public AccountEditorTests() {
            cryptoAccount = new CryptoAccount(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

            dataEditorMock = new Mock<IDataEditor>();
            dataEditorMock
                .Setup(x => x.UpdateAccount(It.IsAny<string>(), It.IsAny<AccountModel>()))
                .Returns<string, AccountModel>((x, y) => string.IsNullOrWhiteSpace(x)? EditorErrorCode.InvalidName : Option.None<EditorErrorCode>());

            dataEditorMock
                .Setup(x => x.UpdateAccountAsync(It.IsAny<string>(), It.IsAny<AccountModel>()))
                .Returns<string, AccountModel>((x, y) => (string.IsNullOrWhiteSpace(x) ? EditorErrorCode.InvalidName : Option.None<EditorErrorCode>()).AsValueTask());
        }

        readonly Mock<IDataEditor> dataEditorMock;
        readonly ICryptoAccount cryptoAccount;

        [Theory]
        [InlineData("accName", "noEncrypt", "password", "email")]
        public void UpdateValuesGotEncrypted(string name, string newName, string password, string email) {

            //arrange
            var (editor, input, expected) = ArrangeTest(newName, password, email);

            //act
            var option = editor.UpdateAccount(name, input);

            //assert
            option.Is(OptionResult.None);
            dataEditorMock.Verify(x => x.UpdateAccount(
                    It.Is<string>(x => x == name),
                    It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))
                ));
            dataEditorMock.VerifyNoOtherCalls();

        }

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

        private (AccountEditor editor, AccountModel input, AccountModel expected) ArrangeTest(string name, string password, string email) {
            var editor = new AccountEditor(dataEditorMock.Object, cryptoAccount);
            var input = new AccountModel(name, password, email);
            var expected = cryptoAccount.Encrypt(input);
            return (editor, input, expected);
        }

    }
}
