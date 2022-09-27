using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Utils;
using PswManager.Utils.Options;
using Xunit;

namespace PswManager.Database.Tests.Generic; 
public abstract class DataReaderGeneric : IDisposable {

    public DataReaderGeneric(ITestDBHandler dbHandler) {
        this.dbHandler = dbHandler.SetUpDefaultValues();
        dataReader = dbHandler.GetDBFactory().GetDataReader();
    }

    readonly IDataReader dataReader;
    readonly ITestDBHandler dbHandler;
    static protected readonly int numValues = 3;

    [Fact]
    public async Task GetOneShouldReturnAsync() {

        //arrange
        AccountModel expected = dbHandler.GetDefaultValues().GetSome(1).First();

        //act
        var actual = await dataReader.GetAccountAsync(expected.Name);

        //assert
        actual.Is(OptionResult.Some);
        AccountEqual(expected, actual.Or(null));

    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetFailure_InvalidName(string name) {

        //act
        var result = await dataReader.GetAccountAsync(name).ConfigureAwait(false);

        //assert
        result.Is(OptionResult.Error);

        Assert.Equal(ReaderErrorCode.InvalidName, GetError(result));

    }

    [Fact]
    public async Task GetFailure_NonExistentName() {

        //arrange
        string name = "gerobhipubihtsiyhrti";

        //act
        var exists = dataReader.AccountExist(name);
        var result = await dataReader.GetAccountAsync(name).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exists);
        result.Is(OptionResult.Error);

        Assert.Equal(ReaderErrorCode.DoesNotExist, GetError(result));

    }

    [Fact]
    public async Task GetAllShouldGetAllAsync() {

        //arrange
        var expectedAccounts = dbHandler.GetDefaultValues().GetAll().ToList();

        //act
        var actual = dataReader.GetAllAccountsAsync();
        List<AccountModel> values = await actual
            .Select(x => x.Or(null))
            .ToList()
            .ConfigureAwait(false);
        values.Sort((x, y) => x.Name.CompareTo(y.Name));

        //assert
        Assert.Equal(expectedAccounts.Count, values.Count);

        Enumerable
            .Range(0, dbHandler.GetDefaultValues().values.Count - 1)
            .ForEach(x => {
                AccountEqual(expectedAccounts[x], values[x]);
            });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetAFewWithIAsyncEnumerator(int num) {

        //arrange
        var expectedAccounts = dbHandler.GetDefaultValues().GetSome(num).ToList();

        //act
        var actual = dataReader.GetAllAccountsAsync();
        List<AccountModel> values = await actual.Select(x => x.Or(null)).Take(num).ToList().ConfigureAwait(false);
        values.Sort((x, y) => x.Name.CompareTo(y.Name));

        //assert
        Assert.Equal(expectedAccounts.Count, values.Count);

        Enumerable
            .Range(0, num)
            .ForEach(x => {
                AccountEqual(expectedAccounts[x], values[x]);
            });

    }

    private static ReaderErrorCode GetError(Option<AccountModel, ReaderErrorCode> option) 
        => option.Match(some => throw new Exception("Option should not be Some."), error => error, () => throw new Exception("Option should not be None."));

    private static void AccountEqual(AccountModel expected, AccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
    }

    public void Dispose() {
        dbHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}
