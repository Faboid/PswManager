using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
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

            dataReaderMock
                .Setup(x => x.GetAllAccounts())
                .Returns(() => ConnectionResultMocks.GenerateInfiniteAccountList());

            dataReaderMock
                .Setup(x => x.GetAllAccountsAsync())
                .Returns(() => Task.FromResult(ConnectionResultMocks.GenerateInfiniteAccountListAsync()));
        }

        readonly Mock<IDataReader> dataReaderMock;
        readonly ICryptoAccount cryptoAccount;

        [Theory]
        [InlineData("someName")]
        public void ReadAccountCallsDBCorrectly(string name) {

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
        public async Task ReadAccountCallsDBCorrectlyAsync(string name) {

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

        [Fact] //if it's iterated, it will deadlock
        public void GetAllAccountsIsNotIterated() {

            //arrange
            AccountReader reader = new(dataReaderMock.Object, cryptoAccount);

            //act
            var result = reader.ReadAllAccounts();
            var listValues = result.Value.Take(50).ToList();

            //assert
            Assert.True(result.Success);
            Assert.True(listValues.Count == 50 && listValues.All(x => x != null));
            dataReaderMock.Verify(x => x.GetAllAccounts());
            dataReaderMock.VerifyNoOtherCalls();

        }

        [Fact] //if it's iterated, it will deadlock
        public async Task GetAllAccountsIsNotIteratedAsync() {

            //arrange
            AccountReader reader = new(dataReaderMock.Object, cryptoAccount);

            //act
            var result = await reader.ReadAllAccountsAsync();
            var listValues = (await result.Value.Take(50)).ToList();

            //assert
            Assert.True(result.Success);
            Assert.True(listValues.Count == 50 && listValues.All(x => x != null));
            dataReaderMock.Verify(x => x.GetAllAccountsAsync());
            dataReaderMock.VerifyNoOtherCalls();

        }

    }
}
