﻿using Moq;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using PswManager.UI.Console.Inner.Interfaces;
using PswManager.UI.Console.Inner;

namespace PswManager.UI.Console.Tests.Inner;
public class AccountReaderTests {

    public AccountReaderTests() {
        cryptoAccount = new CryptoAccountService(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);

        dataReaderMock = new();

        dataReaderMock
            .Setup(x => x.GetAccountAsync(It.Is<string>(x => !string.IsNullOrWhiteSpace(x))))
            .Returns<string>(x =>
                Task.FromResult<Option<IAccountModel, ReaderErrorCode>>(new(AccountModelMocks.GenerateEncryptedFromName(x, cryptoAccount)))
            );

        dataReaderMock
            .Setup(x => x.EnumerateAccountsAsync())
            .Returns(IAsyncEnumerableGenerator.GenerateInfiniteEncryptedAccountListAsync(cryptoAccount));

        reader = new AccountReader(dataReaderMock.Object, cryptoAccount);
    }

    readonly Mock<IDataReader> dataReaderMock;
    readonly ICryptoAccountService cryptoAccount;
    readonly IAccountReader reader;

    [Theory]
    [InlineData("someName")]
    public async Task ReadAccountCallsDBCorrectlyAsync(string name) {

        //arrange
        IAccountModel expected = cryptoAccount.Decrypt(AccountModelMocks.GenerateEncryptedFromName(name, cryptoAccount));

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

    [Fact] //if it's iterated, it will deadlock due to the mock's infinite generation
    public async Task GetAllAccountsIsNotIteratedAsync() {

        //act
        var asyncEnumerable = reader.ReadAllAccountsAsync();
        var listValues = await asyncEnumerable.Take(50).ToListAsync().ConfigureAwait(false);

        //assert
        Assert.True(listValues.Count == 50 && listValues.All(x => x.Match(some => true, error => false, () => false)));
        dataReaderMock.Verify(x => x.EnumerateAccountsAsync());
        dataReaderMock.VerifyNoOtherCalls();

    }

    [Fact]
    public void ReadAllDecryptsCorrectly() {

        //assert
        var expected = AccountModelMocks.GenerateManyEncrypted(cryptoAccount)
            .Take(10)
            .Select(x => cryptoAccount.Decrypt(x))
            .ToList();

        //act
        var result = reader.ReadAllAccounts();
        var ten = result.Take(10).Select(x => x.Or(null)).ToList();

        //assert
        Assert.True(Enumerable.Range(0, 10).All(x => AccountModelAsserts.AssertEqual(expected[x], ten[x])));

    }

    [Fact]
    public async Task ReadAllDecryptsCorrectlyAsync() {

        //assert
        var expected = AccountModelMocks.GenerateManyEncrypted(cryptoAccount)
            .Take(10)
            .Select(x => cryptoAccount.Decrypt(x))
            .ToList();

        //act
        var result = reader.ReadAllAccountsAsync();
        var ten = await result.Take(10).Select(x => x.Or(null)).ToListAsync();

        //assert
        Assert.True(Enumerable.Range(0, 10).All(x => AccountModelAsserts.AssertEqual(expected[x], ten[x])));

    }


}
