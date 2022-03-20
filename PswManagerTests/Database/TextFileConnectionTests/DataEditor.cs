using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;
using PswManagerDatabase.Models;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using System;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataEditor : IDisposable {

        public DataEditor() {
            dbHandler = new TextDatabaseHandler(dbName, 3).SetUpDefaultValues();
            dataEditor = dbHandler.GetDBFactory().GetDataEditor();
        }

        readonly IDataEditor dataEditor;
        readonly TextDatabaseHandler dbHandler;
        const string dbName = "DataEditorTestsDB";

        public static IEnumerable<object[]> UpdateAccountCorrectlyData() {
            var def = new DefaultValues(3);

            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), 
                new AccountModel("newName1", "newPassword1", "newEmail1"),
                new AccountModel("newName1", "newPassword1", "newEmail1")
            };
            yield return new object[] {
                def.GetValue(2, DefaultValues.TypeValue.Name), 
                new AccountModel(null, "randompassword2", null),
                new AccountModel(def.GetValue(2, DefaultValues.TypeValue.Name), "randompassword2", def.GetValue(2, DefaultValues.TypeValue.Email))
            };
            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name), 
                new AccountModel(null, null, "updatedEmail1"),
                new AccountModel(def.GetValue(1, DefaultValues.TypeValue.Name), def.GetValue(1, DefaultValues.TypeValue.Password), "updatedEmail1")
            };
        }

        [Theory]
        [MemberData(nameof(UpdateAccountCorrectlyData))]
        public void UpdateAccountCorrectly(string name, AccountModel newAccount, AccountModel expected) {

            //arrange
            dbHandler.SetUpDefaultValues();

            //act
            var actual = dataEditor.UpdateAccount(name, newAccount).Value;

            //assert
            AccountEqual(expected, actual);

        }

        [Fact]
        public void UpdateAccountFailure_TriedRenamingToExistingAccountName() {

            //arrange
            dbHandler.SetUpDefaultValues();

            string currentName = DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name);
            string newExistingName = DefaultValues.StaticGetValue(1, DefaultValues.TypeValue.Name);
            var newModel = new AccountModel(newExistingName, null, "yoyo@com");

            //act
            var currExists = dataEditor.AccountExist(currentName);
            var newExists = dataEditor.AccountExist(newExistingName);
            var actual = dataEditor.UpdateAccount(currentName, newModel);

            //assert
            Assert.True(currExists);
            Assert.True(newExists);
            Assert.False(actual.Success);

        }

        [Fact]
        public void UpdateAccountFailure_InexistentAccount() {

            //arrange
            string inexistantName = "girhguegjpwkhdu";

            //act
            var exist = dataEditor.AccountExist(inexistantName);
            var actual = dataEditor.UpdateAccount(inexistantName, new AccountModel());

            //assert
            Assert.False(exist);
            Assert.False(actual.Success);

        }

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
}
