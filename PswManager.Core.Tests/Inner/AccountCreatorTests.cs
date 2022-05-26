using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountCreatorTests {

        public AccountCreatorTests() {
            cryptoAccount = new CryptoAccount(ICryptoServiceMocks.GetReverseEncryptor().Object, ICryptoServiceMocks.GetStringToOneCharEncryptor().Object);

            dataCreatorMock = new Mock<IDataCreator>();
            dataCreatorMock
                .Setup(x => x.CreateAccount(It.IsAny<AccountModel>()))
                .Returns<AccountModel>(MockConnectionResult);
            dataCreatorMock
                .Setup(x => x.CreateAccountAsync(It.IsAny<AccountModel>()))
                .Returns<AccountModel>(x => Task.FromResult(MockConnectionResult(x)));
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
            Assert.True(result.Success);
            dataCreatorMock.Verify(x => x.CreateAccount(It.Is<AccountModel>(x => AssertEqual(expected, x))));

        }

        [Theory]
        [InlineData("asyncName", "newpass", "ema@here.com")]
        public async Task AccountObjectGotEncryptedAsync(string name, string password, string email) {

            //arrange
            var (creator, input, expected) = ArrangeTest(name, password, email);

            //act
            var result = await creator.CreateAccountAsync(input);

            //assert
            Assert.True(result.Success);
            dataCreatorMock.Verify(x => x.CreateAccountAsync(It.Is<AccountModel>(x => AssertEqual(expected, x))));

        }

        private static ConnectionResult MockConnectionResult(AccountModel model) {
            bool[] isAnyNullOrEmpty = new bool[] {
                string.IsNullOrWhiteSpace(model.Name),
                string.IsNullOrWhiteSpace(model.Password),
                string.IsNullOrWhiteSpace(model.Email)
            };

            return new ConnectionResult(!isAnyNullOrEmpty.Any(x => x));
        }

        private (AccountCreator creator, AccountModel input, AccountModel expected) ArrangeTest(string name, string password, string email) {
            var creator = new AccountCreator(dataCreatorMock.Object, cryptoAccount);
            var input = new AccountModel(name, password, email);
            var expected = cryptoAccount.Encrypt(input);
            return (creator, input, expected);
        }

        private static bool AssertEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
            return true;
        }

    }
}
