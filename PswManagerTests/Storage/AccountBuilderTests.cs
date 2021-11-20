using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage {

    [Collection("TestHelperCollection")]
    public class AccountBuilderTests {

        [Fact]
        public void SearchShouldFind() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);
            int? actual;
            int expected = 2;

            //act
            actual = builder.Search(TestsHelper.defaultValues.GetValue(2, DefaultValues.TypeValue.Name));

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void SearchInexistentGetNull() {

            //arrange
            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);
            int? actual;
            int? expected = null;

            //act
            actual = builder.Search("thisnamedoesn'texist");

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);
            string name = TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            (string name, string password, string email) expected = (
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name), 
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Password), 
                TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );
            (string name, string password, string email) actual;

            //act
            actual = builder.GetOne(name);
            actual.password = TestsHelper.passCryptoString.Decrypt(actual.password);
            actual.email = TestsHelper.emaCryptoString.Decrypt(actual.email);

            //assert
            Assert.Equal(expected, actual);

        }

        //todo - fix this mess of a test
        [Theory]
        [InlineData("defaultName1", "newName1", "newPassword1", "newEmail1", "newName1 newPassword1 newEmail1")]
        [InlineData("defaultName2", null, "randompassword2", null, "defaultName2 randompassword2 defaultEmail2")]
        [InlineData("defaultName1", null, null, "updatedEmail1", "defaultName1 defaultPassword1 updatedEmail1")]
        public void EditOneCorrectly(string name, string newName, string newPassword, string newEmail, string expectedstring) {

            //arrange
            TestsHelper.SetUpDefault();
            if(newPassword is not null) {
                newPassword = TestsHelper.passCryptoString.Encrypt(newPassword);
            }
            if(newEmail is not null) {
                newEmail = TestsHelper.emaCryptoString.Encrypt(newEmail);
            }

            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);

            var splitstrings = expectedstring.Split(' ');
            (string name, string password, string email) expected = (splitstrings[0], splitstrings[1], splitstrings[2]);
            (string name, string password, string email) actual;

            //act
            builder.EditOne(name, newName, newPassword, newEmail);
            actual = builder.GetOne(newName ?? name);
            actual.password = TestsHelper.passCryptoString.Decrypt(actual.password);
            actual.email = TestsHelper.emaCryptoString.Decrypt(actual.email);

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void DeleteOneCorrectly() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.paths);
            string name = TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = builder.Search(name) is not null;
            builder.DeleteOne(name);

            //assert
            Assert.True(exists);
            Assert.Null(builder.Search(name));

        }

    }
}
