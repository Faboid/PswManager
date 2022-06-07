using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountCreatorTests {

        public AccountCreatorTests() {
            cryptoAccount = new CryptoAccount(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

            dataCreatorMock = new Mock<IDataCreator>();
            dataCreatorMock
                .Setup(x => x.CreateAccount(It.IsAny<AccountModel>()))
                .Returns<AccountModel>(OptionMocks.ValidateValues);
            dataCreatorMock
                .Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>()))
                .Returns<AccountModel>(x => Task.FromResult(OptionMocks.ValidateValues(x)));
        }

        //since the purpose of this class is encrypting the password & email,
        //and then passing the values down to the database, there's no need to test the database itself.
        readonly Mock<IDataCreator> dataCreatorMock;

        //a single version to produce consistent results
        readonly ICryptoAccount cryptoAccount;

        [Theory]
        [InlineData("nameHere", "passPass", "emaema")]
        public void AccountObjectGotEncrypted(string name, string password, string email) {

            //arrange
            var (creator, input, expected) = ArrangeTest(name, password, email);

            //act
            var result = creator.CreateAccount(input);

            //assert
            Assert.True(OptionToSuccess(result));
            dataCreatorMock.Verify(x => x.CreateAccount(It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))));
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
            Assert.True(OptionToSuccess(result));
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
            var result = creator.CreateAccount(input);
            var resultAsync = await creator.CreateAccountAsync(input);

            //assert
            Assert.False(OptionToSuccess(result));
            Assert.False(OptionToSuccess(resultAsync));
            dataCreatorMock.VerifyNoOtherCalls();

        }

        private static bool OptionToSuccess(Option<CreatorErrorCode> option) => option.Match(error => false, () => true);

        private (AccountCreator creator, AccountModel input, AccountModel expected) ArrangeTest(string name, string password, string email) {
            var creator = new AccountCreator(dataCreatorMock.Object, cryptoAccount);
            var input = new AccountModel(name, password, email);
            var expected = cryptoAccount.Encrypt(input);
            return (creator, input, expected);
        }

    }
}
