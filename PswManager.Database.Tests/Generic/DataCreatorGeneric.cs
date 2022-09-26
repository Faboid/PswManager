﻿using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using Xunit;

namespace PswManager.Database.Tests.Generic; 
public abstract class DataCreatorGeneric : IDisposable {

    public DataCreatorGeneric(ITestDBHandler dbHandler) {
        dataCreator = dbHandler.GetDBFactory().GetDataCreator();
        this.dbHandler = dbHandler;
        dbHandler.SetUpDefaultValues();
    }

    private readonly IDataCreator dataCreator;
    private readonly ITestDBHandler dbHandler;
    protected static readonly int numValues = 1;

    [Fact]
    public async Task CreateAccountCorrectlyAsync() {

        //arrange
        var account = new AccountModel("validAccountName", "rigteuwokgteuyh", "here@email.it");

        //act
        var exist = await dataCreator.AccountExistAsync(account.Name).ConfigureAwait(false);
        var result = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exist);
        Assert.True(OptionToSuccess(result));
        Assert.Equal(AccountExistsStatus.Exist, await dataCreator.AccountExistAsync(account.Name).ConfigureAwait(false));

    }

    [Fact]
    public async Task CreateAccountFailure_AlreadyExists() {

        //arrange
        var account = new AccountModel(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), "password", "email@here.com");

        //act
        var exist = dataCreator.AccountExist(account.Name);
        var result = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.Exist, exist);
        Assert.False(OptionToSuccess(result));


        Assert.Equal(CreatorErrorCode.AccountExistsAlready, result.Or(default));

    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public async void CreateAccountFailure_InvalidName(string name) {

        //arrange
        var account = new AccountModel(name, "passhere", "ema@here.com");

        //act
        var result = await dataCreator.CreateAccountAsync(account);

        //assert
        Assert.False(OptionToSuccess(result));


        Assert.Equal(CreatorErrorCode.InvalidName, result.Or(default));

    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateAccountFailure_InvalidPassword(string password) {

        //arrange
        var account = new AccountModel("veryvalidunusedName", password, "valid@email.com");

        //act
        var result = await dataCreator.CreateAccountAsync(account);

        //assert
        Assert.False(OptionToSuccess(result));

        Assert.Equal(CreatorErrorCode.MissingPassword, result.Or(default));

    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateAccountFailure_InvalidEmail(string email) {

        //arrange
        var account = new AccountModel("validNamenongriurh", "somepass", email);

        //act
        var result = await dataCreator.CreateAccountAsync(account);

        //assert
        Assert.False(OptionToSuccess(result));


        Assert.Equal(CreatorErrorCode.MissingEmail, result.Or(default));

    }

    private static bool OptionToSuccess(Option<CreatorErrorCode> option) => option.Match(error => false, () => true);

    public void Dispose() {
        dbHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}
