using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;
using PswManagerDatabase.Models;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataEditor {

        public static IEnumerable<object[]> UpdateAccountCorrectlyData() {
            var def = new DefaultValues(5);

            yield return new object[] {
                def.GetValue(1, DefaultValues.TypeValue.Name),
                new AccountModel("newName1", "newPassword1", "newEmail1"),
                new AccountModel("newName1", "newPassword1", "newEmail1")
            };
            yield return new object[] {
                def.GetValue(2, DefaultValues.TypeValue.Name),
                new AccountModel("   ", "randompassword2", ""),
                new AccountModel(def.GetValue(2, DefaultValues.TypeValue.Name), "randompassword2", def.GetValue(2, DefaultValues.TypeValue.Email))
            };
            yield return new object[] {
                def.GetValue(2, DefaultValues.TypeValue.Name),
                new AccountModel("newNameHere", "    ", null),
                new AccountModel("newNameHere", def.GetValue(2, DefaultValues.TypeValue.Password), def.GetValue(2, DefaultValues.TypeValue.Email))
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
            var dataEditor = new MemoryDBHandler()
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataEditor();

            //act
            var actual = dataEditor.UpdateAccount(name, newAccount).Value;

            //assert
            AssertAccountEqual(expected, actual);

        }

        [Fact]
        public void UpdateAccountFailure_InexistentAccount() {

            //arrange
            var dataEditor = new MemoryDBHandler()
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataEditor();
            string inexistantName = "girhguegjpwkhdu";

            //act
            var exist = dataEditor.AccountExist(inexistantName);
            var actual = dataEditor.UpdateAccount(inexistantName, new AccountModel());

            //assert
            Assert.False(exist);
            Assert.False(actual.Success);

        }

        private static void AssertAccountEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
        }

    }
}
