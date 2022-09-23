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
    public void GetOneShouldReturn() {

        //arrange
        AccountModel expected = new(
            dbHandler.GetDefaultValues().GetValue(1, DefaultValues.TypeValue.Name),
            dbHandler.GetDefaultValues().GetValue(1, DefaultValues.TypeValue.Password),
            dbHandler.GetDefaultValues().GetValue(1, DefaultValues.TypeValue.Email)
            );

        //act
        var actual = dataReader.GetAccount(expected.Name);

        //assert
        actual.Is(OptionResult.Some);
        AccountEqual(expected, actual.Or(null));

    }

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
        var result = dataReader.GetAccount(name);
        var resultAsync = await dataReader.GetAccountAsync(name).ConfigureAwait(false);

        //assert
        result.Is(OptionResult.Error);
        resultAsync.Is(OptionResult.Error);

        Assert.Equal(ReaderErrorCode.InvalidName, GetError(result));
        Assert.Equal(ReaderErrorCode.InvalidName, GetError(resultAsync));

    }

    [Fact]
    public async Task GetFailure_NonExistentName() {

        //arrange
        string name = "gerobhipubihtsiyhrti";

        //act
        var exists = dataReader.AccountExist(name);
        var result = dataReader.GetAccount(name);
        var resultAsync = await dataReader.GetAccountAsync(name).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exists);
        result.Is(OptionResult.Error);
        resultAsync.Is(OptionResult.Error);

        Assert.Equal(ReaderErrorCode.DoesNotExist, GetError(result));
        Assert.Equal(ReaderErrorCode.DoesNotExist, GetError(resultAsync));

    }

    [Fact]
    public void GetAllShouldGetAll() {

        //arrange
        var expectedAccounts = dbHandler.GetDefaultValues().GetAll().ToList();

        //act
        var actual = dataReader.GetAllAccounts();
        var values = actual.Or(null)
            .Select(x => x.Or(null))
            .OrderBy(x => x.Name)
            .ToList();

        //assert
        Assert.True(MatchToBool(actual));
        Assert.Equal(expectedAccounts.Count, values.Count);

        Enumerable
            .Range(0, dbHandler.GetDefaultValues().values.Count - 1)
            .ForEach(x => {
                AccountEqual(expectedAccounts[x], values[x]);
            });
    }

    [Fact]
    public async Task GetAllShouldGetAllAsync() {

        //arrange
        var expectedAccounts = dbHandler.GetDefaultValues().GetAll().ToList();

        //act
        var actual = await dataReader.GetAllAccountsAsync().ConfigureAwait(false);
        List<AccountModel> values = await actual
            .Or(null)
            .Select(x => x.Or(null))
            .ToList()
            .ConfigureAwait(false);
        values.Sort((x, y) => x.Name.CompareTo(y.Name));

        //assert
        Assert.True(MatchToBool(actual));
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
    public void GetAFewWithIEnumerator(int num) {

        //arrange
        var expectedAccounts = dbHandler.GetDefaultValues().GetSome(num).ToList();

        //act
        var actual = dataReader.GetAllAccounts();
        var values = actual
            .Or(null)
            .Select(x => x.Or(null))
            .Where(x => x != null)
            .OrderBy(x => x.Name)
            .Take(num)
            .ToList();

        //assert
        Assert.True(MatchToBool(actual));
        Assert.Equal(expectedAccounts.Count, values.Count);

        Enumerable
            .Range(0, num)
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
        var actual = await dataReader.GetAllAccountsAsync().ConfigureAwait(false);
        List<AccountModel> values = await actual.Or(null).Select(x => x.Or(null)).Take(num).ToList().ConfigureAwait(false);
        values.Sort((x, y) => x.Name.CompareTo(y.Name));

        //assert
        Assert.True(MatchToBool(actual));
        Assert.Equal(expectedAccounts.Count, values.Count);

        Enumerable
            .Range(0, num)
            .ForEach(x => {
                AccountEqual(expectedAccounts[x], values[x]);
            });

    }

    private static bool MatchToBool<TValue, TError>(Option<TValue, TError> option) => option.Match(some => true, error => false, () => false);

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
