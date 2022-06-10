using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.Options;
using Xunit;

namespace PswManager.Database.Tests.Generic {
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

        private bool firstRun = true;

        [Theory]
        [MemberData(nameof(UpdateAccountCorrectlyData))]
        public void UpdateAccountCorrectly(string name, AccountModel newAccount, AccountModel expected) {

            //arrange
            if(firstRun) {
                dbHandler.SetUpDefaultValues();
                firstRun = false;
            }

            //act
            var actual = dataEditor.UpdateAccount(name, newAccount);
            var updated = dataReader.GetAccount(string.IsNullOrWhiteSpace(newAccount.Name) ? name : newAccount.Name);

            //assert
            actual.Is(OptionResult.None);
            Assert.NotNull(updated.Or(null));
            AssertAccountEqual(expected, updated.Or(null));

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
            var option = await dataEditor.UpdateAccountAsync(name, newAccount).ConfigureAwait(false);
            var updated = await dataReader.GetAccountAsync(string.IsNullOrWhiteSpace(newAccount.Name) ? name : newAccount.Name).ConfigureAwait(false);

            //assert
            option.Is(OptionResult.None);
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
            var result = dataEditor.UpdateAccount(inexistantName, model);
            var resultAsync = await dataEditor.UpdateAccountAsync(inexistantName, model);

            //assert
            Assert.Equal(AccountExistsStatus.NotExist, exist);
            result.Is(OptionResult.Some);
            resultAsync.Is(OptionResult.Some);

            Assert.Equal(EditorErrorCode.DoesNotExist, result.Or(default));
            Assert.Equal(EditorErrorCode.DoesNotExist, resultAsync.Or(default));

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
            var result = dataEditor.UpdateAccount(currentName, newModel);
            var resultAsync = await dataEditor.UpdateAccountAsync(currentName, newModel).ConfigureAwait(false);

            //assert
            Assert.Equal(AccountExistsStatus.Exist, currExists);
            Assert.Equal(AccountExistsStatus.Exist, newExists);

            result.Is(OptionResult.Some);
            resultAsync.Is(OptionResult.Some);

            Assert.Equal(EditorErrorCode.NewNameExistsAlready, result.Or(default));
            Assert.Equal(EditorErrorCode.NewNameExistsAlready, resultAsync.Or(default));

        }

        private static void AssertAccountEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
