using Moq;
using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Inner.Interfaces;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using Xunit;

namespace PswManager.Core.Tests.Inner; 
public class AccountReaderTests {

    public AccountReaderTests() {
        cryptoAccount = new CryptoAccount(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

        dataReaderMock = new();

        dataReaderMock
            .Setup(x => x.GetAccount(It.Is<string>(x => !string.IsNullOrWhiteSpace(x))))
            .Returns<string>(x => AccountModelMocks.GenerateEncryptedFromName(x, cryptoAccount));

        dataReaderMock
            .Setup(x => x.GetAccountAsync(It.Is<string>(x => !string.IsNullOrWhiteSpace(x))))
            .Returns<string>(x => 
                ValueTask.FromResult<Option<AccountModel, ReaderErrorCode>>(AccountModelMocks.GenerateEncryptedFromName(x, cryptoAccount))
            );

        dataReaderMock
            .Setup(x => x.GetAllAccounts())
            .Returns(() => OptionMocks.GenerateInfiniteEncryptedAccountList(cryptoAccount));

        dataReaderMock
            .Setup(x => x.GetAllAccountsAsync())
            .Returns(() => Task.FromResult(OptionMocks.GenerateInfiniteEncryptedAccountListAsync(cryptoAccount)));

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
        var option = reader.ReadAccount(name);

        //assert
        Assert.True(option.Match(some => true, error => false, () => false));
        AccountModelAsserts.AssertEqual(expected, option.Or(null));
        dataReaderMock.Verify(x => x.GetAccount(It.Is<string>(x => x == name)));
        dataReaderMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData("someName")]
    public async Task ReadAccountCallsDBCorrectlyAsync(string name) {

        //arrange
        AccountModel expected = cryptoAccount.Decrypt(AccountModelMocks.GenerateEncryptedFromName(name, cryptoAccount));

        //act
        var option = await reader.ReadAccountAsync(name);

        //assert
        Assert.True(option.Match(some => true, error => false, () => false));
        AccountModelAsserts.AssertEqual(expected, option.Or(null));
        dataReaderMock.Verify(x => x.GetAccountAsync(It.Is<string>(x => x == name)));
        dataReaderMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task NoCallsIfInvalidName(string name) {

        //act
        var option = reader.ReadAccount(name);
        var optionAsync = await reader.ReadAccountAsync(name);

        //assert
        Assert.False(option.Match(some => true, error => false, () => false));
        Assert.False(optionAsync.Match(some => true, error => false, () => false));
        dataReaderMock.VerifyNoOtherCalls();

    }

    [Fact] //if it's iterated, it will deadlock
    public void GetAllAccountsIsNotIterated() {

        //act
        var option = reader.ReadAllAccounts();
        var listValues = option.Or(null).Take(50).ToList();

        //assert
        Assert.True(option.Match(some => true, error => false, () => false));
        Assert.True(listValues.Count == 50 && listValues.All(x => x.Match(some => true, error => false, () => false)));
        dataReaderMock.Verify(x => x.GetAllAccounts());
        dataReaderMock.VerifyNoOtherCalls();

    }

    [Fact] //if it's iterated, it will deadlock
    public async Task GetAllAccountsIsNotIteratedAsync() {

        //act
        var option = await reader.ReadAllAccountsAsync();
        var listValues = await option.Or(null).Take(50).ToList().ConfigureAwait(false);

        //assert
        Assert.True(option.Match(some => true, error => false, () => false));
        Assert.True(listValues.Count == 50 && listValues.All(x => x.Match(some => true, error => false, () => false)));
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
        var ten = result.Or(null).Take(10).Select(x => x.Or(null)).ToList();

        //assert
        Assert.True(result.Match(some => true, error => false, () => false));
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
        var ten = await result.Or(null).Take(10).Select(x => x.Or(null)).ToList();

        //assert
        Assert.True(result.Match(some => true, error => false, () => false));
        Assert.True(Enumerable.Range(0, 10).All(x => AccountModelAsserts.AssertEqual(expected[x], ten[x])));

    }


}
