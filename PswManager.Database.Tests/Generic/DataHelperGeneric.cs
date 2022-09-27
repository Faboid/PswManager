using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using Xunit;

namespace PswManager.Database.Tests.Generic;
public abstract class DataHelperGeneric : IDisposable {

    public DataHelperGeneric(ITestDBHandler dbHandler) {
        this.dbHandler = dbHandler.SetUpDefaultValues();
        dataHelper = dbHandler.GetDBFactory().GetDataHelper();
    }

    readonly IDataHelper dataHelper;
    readonly ITestDBHandler dbHandler;
    protected static readonly int numValues = 1;

    public static IEnumerable<object[]> AccountExistTestsData() {
        static object[] NewObj(string? name, AccountExistsStatus expected) => new object[] { name!, expected };

        yield return NewObj(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), AccountExistsStatus.Exist);
        yield return NewObj("rtoehognrkljnwigurehvbonolbneughwonko", AccountExistsStatus.NotExist);
        yield return NewObj("", AccountExistsStatus.InvalidName);
        yield return NewObj("   ", AccountExistsStatus.InvalidName);
        yield return NewObj(null, AccountExistsStatus.InvalidName);

    }

    [Theory]
    [MemberData(nameof(AccountExistTestsData))]
    public void AccountExist(string? name, AccountExistsStatus expected) {

        Assert.Equal(expected, dataHelper.AccountExist(name));
    }

    [Theory]
    [MemberData(nameof(AccountExistTestsData))]
    public async Task AccountExistsAsync(string? name, AccountExistsStatus expected) {

        Assert.Equal(expected, await dataHelper.AccountExistAsync(name));
    }

    public void Dispose() {
        dbHandler.Dispose();
        GC.SuppressFinalize(this);
    }

}
