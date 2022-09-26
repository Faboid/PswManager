using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils.Options;
using Xunit;

namespace PswManager.Database.Tests.Generic; 
public abstract class DataDeleterGeneric : IDisposable {

    public DataDeleterGeneric(ITestDBHandler dbHandler) {
        this.dbHandler = dbHandler.SetUpDefaultValues();
        dataDeleter = dbHandler.GetDBFactory().GetDataDeleter();
    }

    readonly IDataDeleter dataDeleter;
    readonly ITestDBHandler dbHandler;
    static protected readonly int numValues = 1;

    [Fact]
    public async Task DeleteAccountCorrectlyAsynchronously() {

        //arrange
        string name = dbHandler.GetDefaultValues().GetValue(0, DefaultValues.TypeValue.Name);

        //act
        var exists = await dataDeleter.AccountExistAsync(name).ConfigureAwait(false);
        var result = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.Exist, exists);
        result.Is(OptionResult.None);
        Assert.Equal(AccountExistsStatus.NotExist, await dataDeleter.AccountExistAsync(name).ConfigureAwait(false));

    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task DeleteFailure_InvalidName(string name) {

        //act
        var result = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

        //assert
        result.Is(OptionResult.Some);

        Assert.Equal(DeleterErrorCode.InvalidName, result.Or(default));

    }

    [Fact]
    public async Task DeleteFailure_NonExistentName() {

        //arrange
        string name = "gerobhipubihtsiyhrti";

        //act
        var exists = dataDeleter.AccountExist(name);
        var result = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exists);
        result.Is(OptionResult.Some);


        Assert.Equal(DeleterErrorCode.DoesNotExist, result.Or(default));

    }

    public void Dispose() {
        dbHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}
