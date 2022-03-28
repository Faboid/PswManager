using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.Generic {
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

        [Theory]
        [MemberData(nameof(UpdateAccountCorrectlyData))]
        public void UpdateAccountCorrectly(string name, AccountModel newAccount, AccountModel expected) {

            //act
            var actual = dataEditor.UpdateAccount(name, newAccount);

            //assert
            Assert.True(actual.Success);
            AssertAccountEqual(expected, actual.Value);

        }

        [Fact]
        public void UpdateAccountFailure_InexistentAccount() {

            //arrange
            string inexistantName = "guioehgioneiopghby";

            //act
            var exist = dataEditor.AccountExist(inexistantName);
            var result = dataEditor.UpdateAccount(inexistantName, new("somenewName", "newPass", "newEma"));

            //assert
            Assert.False(exist);
            Assert.False(result.Success);

        }

        [Fact]
        public void UpdateAccountFailure_TriedRenamingToExistingAccountName() {

            //arrange
            string currentName = dbHandler.GetDefaultValues().GetValue(4, DefaultValues.TypeValue.Name);
            string newExistingName = dbHandler.GetDefaultValues().GetValue(5, DefaultValues.TypeValue.Name);
            var newModel = new AccountModel(newExistingName, null, "yoyo@ema.com");

            //act
            var currExists = dataEditor.AccountExist(currentName);
            var newExists = dataEditor.AccountExist(newExistingName);
            var result = dataEditor.UpdateAccount(currentName, newModel);

            //assert
            Assert.True(currExists);
            Assert.True(newExists);
            Assert.False(result.Success);

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
