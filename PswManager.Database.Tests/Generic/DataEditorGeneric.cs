using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.Options;
using Xunit;

namespace PswManager.Database.Tests.Generic;
public abstract class DataEditorGeneric : IDisposable {
    public DataEditorGeneric(ITestDBHandler dbHandler) {
        this.dbHandler = dbHandler.SetUpDefaultValues();
        dataEditor = dbHandler.GetDBFactory().GetDataEditor();
        dataReader = dbHandler.GetDBFactory().GetDataReader();
    }

    readonly IDataEditor dataEditor;
    readonly IDataReader dataReader;
    readonly ITestDBHandler dbHandler;
    readonly static protected int numValues = 6;

    public static IEnumerable<object[]> UpdateAccountCorrectlyData() {
        static object[] AddData(string name, AccountModel newAccount, AccountModel expected)
            => new object[] { name, newAccount, expected };

        var def = new DefaultValues(4);

        yield return AddData(
            def.GetValue(0, DefaultValues.TypeValue.Name),
            new("newName1", "newPassword1", "newEmail1"),
            new("newName1", "newPassword1", "newEmail1")
        );

        yield return AddData(
            def.GetValue(1, DefaultValues.TypeValue.Name),
            new("    ", "randompassword2", ""),
            new(def.GetValue(1, DefaultValues.TypeValue.Name), "randompassword2", def.GetValue(1, DefaultValues.TypeValue.Email))
        );

        yield return AddData(
            def.GetValue(2, DefaultValues.TypeValue.Name),
            new("newNameHere", "    ", null),
            new("newNameHere", def.GetValue(2, DefaultValues.TypeValue.Password), def.GetValue(2, DefaultValues.TypeValue.Email))
        );

        yield return AddData(
            def.GetValue(3, DefaultValues.TypeValue.Name),
            new(null, null, "updatedEmail1"),
            new(def.GetValue(3, DefaultValues.TypeValue.Name), def.GetValue(3, DefaultValues.TypeValue.Password), "updatedEmail1")
        );
    }

    private bool firstAsyncRun = true;

    [Theory]
    [MemberData(nameof(UpdateAccountCorrectlyData))]
    public async Task UpdateAccountCorrectlyAsync(string name, AccountModel newAccount, AccountModel expected) {

        //arrange
        if(firstAsyncRun) {
            dbHandler.SetUpDefaultValues();
            firstAsyncRun = false;
        }

        //act
        var result = await dataEditor.UpdateAccountAsync(name, newAccount).ConfigureAwait(false);
        var updated = await dataReader.GetAccountAsync(string.IsNullOrWhiteSpace(newAccount.Name) ? name : newAccount.Name).ConfigureAwait(false);

        //assert
        Assert.Equal(EditorResponseCode.Success, result);
        Assert.NotNull(updated.Or(null));
        AssertAccountEqual(expected, updated.Or(null));

    }

    [Fact]
    public async Task UpdateAccountFailure_InexistentAccount() {

        //arrange
        string inexistantName = "guioehgioneiopghby";
        AccountModel model = new("somenewName", "newPass", "newEma");

        //act
        var exist = dataEditor.AccountExist(inexistantName);
        var result = await dataEditor.UpdateAccountAsync(inexistantName, model);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exist);
        Assert.Equal(EditorResponseCode.DoesNotExist, result);

    }

    [Fact]
    public async Task UpdateAccountFailure_TriedRenamingToExistingAccountName() {

        //arrange
        string currentName = dbHandler.GetDefaultValues().GetValue(4, DefaultValues.TypeValue.Name);
        string newExistingName = dbHandler.GetDefaultValues().GetValue(5, DefaultValues.TypeValue.Name);
        var newModel = new AccountModel(newExistingName, null, "yoyo@ema.com");

        //act
        var currExists = dataEditor.AccountExist(currentName);
        var newExists = dataEditor.AccountExist(newExistingName);
        var result = await dataEditor.UpdateAccountAsync(currentName, newModel).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.Exist, currExists);
        Assert.Equal(AccountExistsStatus.Exist, newExists);
        Assert.Equal(EditorResponseCode.NewNameExistsAlready, result);

    }

    private static void AssertAccountEqual(IReadOnlyAccountModel expected, IReadOnlyAccountModel actual) {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
    }

    public void Dispose() {
        dbHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}
