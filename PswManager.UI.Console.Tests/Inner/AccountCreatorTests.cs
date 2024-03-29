﻿using Moq;
using PswManager.Core.Services;
using PswManager.Core.Tests.Asserts;
using PswManager.Core.Tests.Mocks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using PswManager.UI.Console.Inner;

namespace PswManager.UI.Console.Tests.Inner;
public class AccountCreatorTests {

    public AccountCreatorTests() {
        cryptoAccount = new CryptoAccountService(ICryptoServiceMocks.GetReverseCryptor().Object, ICryptoServiceMocks.GetSummingCryptor().Object);
        dataCreatorMock = DataCreatorMock.GetValidatorMock();
    }

    //since the purpose of this class is encrypting the password & email,
    //and then passing the values down to the database, there's no need to test the database itself.
    readonly Mock<IDataCreator> dataCreatorMock;

    //a single version to produce consistent results
    readonly ICryptoAccountService cryptoAccount;

    [Theory]
    [InlineData("asyncName", "newpass", "ema@here.com")]
    public void AccountObjectGotEncrypted(string name, string password, string email) {

        //arrange
        var (creator, input, expected) = ArrangeTest(name, password, email);

        //act
        var result = creator.CreateAccount(input);

        //assert
        Assert.Equal(CreatorResponseCode.Success, result);
        dataCreatorMock.Verify(x => x.CreateAccountAsync(It.Is<AccountModel>(x => AccountModelAsserts.AssertEqual(expected, x))));
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
        Assert.Equal(CreatorResponseCode.Success, result);
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
        var actual = creator.CreateAccount(input);
        var actualAsync = await creator.CreateAccountAsync(input);

        //assert
        Assert.Equal(CreatorResponseCode.InvalidName, actual);
        Assert.Equal(CreatorResponseCode.InvalidName, actualAsync);
        dataCreatorMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task MethodCallsArePure() {

        //arrange
        var expected = new AccountModel("SomeName", "SomePassword", "SomeEmail");
        var actual = new AccountModel(expected.Name, expected.Password, expected.Email);
        var sut = new AccountCreator(dataCreatorMock.Object, cryptoAccount);

        //act
        _ = await sut.CreateAccountAsync(actual);

        //assert
        AccountModelAsserts.AssertEqual(expected, actual);

    }

    private (AccountCreator creator, IAccountModel input, IAccountModel expected) ArrangeTest(string name, string password, string email) {
        var creator = new AccountCreator(dataCreatorMock.Object, cryptoAccount);
        var input = new AccountModel(name, password, email);
        var expected = cryptoAccount.Encrypt(input);
        return (creator, input, expected);
    }

}
