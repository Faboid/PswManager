using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using Xunit;

namespace PswManager.Core.Tests.Inner {
    public class AccountReaderTests {

        public AccountReaderTests() {
            cryptoAccount = new CryptoAccount(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

            dataReaderMock = new();

            dataReaderMock
                .Setup(x => x.GetAccount(It.Is<string>(x => !string.IsNullOrWhiteSpace(x))))
                .Returns<string>(x => ConnectionResultMocks.GenerateEncryptedSuccessFromName(x, cryptoAccount));

            dataReaderMock
                .Setup(x => x.GetAccountAsync(It.Is<string>(x => !string.IsNullOrWhiteSpace(x))))
                .Returns<string>(x => 
                    ValueTask.FromResult(ConnectionResultMocks.GenerateEncryptedSuccessFromName(x, cryptoAccount))
                );
        }

        readonly Mock<IDataReader> dataReaderMock;
        readonly ICryptoAccount cryptoAccount;

        [Theory]
        [InlineData("someName")]
        public void DeleteAccountCallsDBCorrectly(string name) {

            //arrange
            AccountReader reader = new(dataReaderMock.Object, cryptoAccount);
            AccountModel expected = cryptoAccount.Decrypt(AccountModelMocks.GenerateEncryptedFromName(name, cryptoAccount));

            //act
            var result = reader.ReadAccount(name);

            //assert
            Assert.True(result.Success);
            AccountModelAsserts.AssertEqual(expected, result.Value);
            dataReaderMock.Verify(x => x.GetAccount(It.Is<string>(x => x == name)));
            dataReaderMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData("someName")]
        public async Task DeleteAccountCallsDBCorrectlyAsync(string name) {

            //arrange
            AccountReader reader = new(dataReaderMock.Object, cryptoAccount);
            AccountModel expected = cryptoAccount.Decrypt(AccountModelMocks.GenerateEncryptedFromName(name, cryptoAccount));

            //act
            var result = await reader.ReadAccountAsync(name);

            //assert
            Assert.True(result.Success);
            AccountModelAsserts.AssertEqual(expected, result.Value);
            dataReaderMock.Verify(x => x.GetAccountAsync(It.Is<string>(x => x == name)));
            dataReaderMock.VerifyNoOtherCalls();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task NoCallsIfInvalidName(string name) {

            //arrange
            AccountReader reader = new(dataReaderMock.Object, cryptoAccount);

            //act
            var result = reader.ReadAccount(name);
            var resultAsync = await reader.ReadAccountAsync(name);

            //assert
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);
            dataReaderMock.VerifyNoOtherCalls();

        }

    }
}
