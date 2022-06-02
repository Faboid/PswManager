using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Inner.Interfaces;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
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
                .Returns(() => ConnectionResultMocks.GenerateInfiniteEncryptedAccountList(cryptoAccount));

            dataReaderMock
                .Setup(x => x.GetAllAccountsAsync())
                .Returns(() => Task.FromResult(ConnectionResultMocks.GenerateInfiniteEncryptedAccountListAsync(cryptoAccount)));

            reader = new AccountReader(dataReaderMock.Object, cryptoAccount);
        }

        readonly Mock<IDataReader> dataReaderMock;
        readonly ICryptoAccount cryptoAccount;
        readonly IAccountReader reader;

        [Theory]
        [InlineData("someName")]
        public void ReadAccountCallsDBCorrectly(string name) {

            //arrange
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

            //act
            var result = await reader.ReadAllAccountsAsync();
            var listValues = await result.Value.Take(50).ToList();

            //assert
            Assert.True(result.Success);
            Assert.True(listValues.Count == 50 && listValues.All(x => x != null));
            dataReaderMock.Verify(x => x.GetAllAccountsAsync());
            dataReaderMock.VerifyNoOtherCalls();

        }

        [Fact]
        public void ReadAllDecryptsCorrectly() {

            //assert
            var expected = ConnectionResultMocks
                .GenerateInfiniteEncryptedAccountList(cryptoAccount)
                .Value
                .Take(10)
                .Select(x => cryptoAccount.Decrypt(x.Value))
                .ToList();

            //act
            var result = reader.ReadAllAccounts();
            var ten = result.Value.Take(10).Select(x => x.Value).ToList();

            //assert
            Assert.True(result.Success);
            Assert.True(Enumerable.Range(0, 10).All(x => AccountModelAsserts.AssertEqual(expected[x], ten[x])));

        }

        [Fact]
        public async Task ReadAllDecryptsCorrectlyAsync() {

            //assert
            var expected = ConnectionResultMocks
                .GenerateInfiniteEncryptedAccountList(cryptoAccount)
                .Value
                .Take(10)
                .Select(x => cryptoAccount.Decrypt(x.Value))
                .ToList();

            //act
            var result = await reader.ReadAllAccountsAsync().ConfigureAwait(false);
            var ten = await result.Value.Take(10).Select(x => x.Value).ToList();

            //assert
            Assert.True(result.Success);
            Assert.True(Enumerable.Range(0, 10).All(x => AccountModelAsserts.AssertEqual(expected[x], ten[x])));

        }


    }
}
