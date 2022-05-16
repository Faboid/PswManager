using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Tests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PswManager.Tests.Database.Generic {
    public abstract class DataEditorGeneric : IDisposable {
        public DataEditorGeneric(ITestDBHandler dbHandler) {
            this.dbHandler = dbHandler.SetUpDefaultValues();
            dataEditor = dbHandler.GetDBFactory().GetDataEditor();
        }

        readonly IDataEditor dataEditor;
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

            //assert
            Assert.True(actual.Success);
            AssertAccountEqual(expected, actual.Value);

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
            var actual = await dataEditor.UpdateAccountAsync(name, newAccount).ConfigureAwait(false);

            //assert
            Assert.True(actual.Success);
            AssertAccountEqual(expected, actual.Value);

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
            Assert.False(exist);
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

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
            Assert.True(currExists);
            Assert.True(newExists);
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

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
