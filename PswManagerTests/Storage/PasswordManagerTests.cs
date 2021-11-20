using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PswManagerLibrary.Exceptions;
using PswManagerTests;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Storage {
    
    [Collection("TestHelperCollection")]
    public class PasswordManagerTests {

        [Fact]
        public void CreatePasswordSuccess() {

            //arrange
            var manager = TestsHelper.pswManager;
            string name = "name";
            string password = "psw";
            string email = "ema@il";

            //act
            manager.CreatePassword(name, password, email);

            //assert
            Assert.True(manager.AccountExist(name));

        }

        [Fact]
        public void GetPasswordSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.pswManager;
            string expected = TestsHelper.defaultValues.values[1];

            //act
            var actual = manager.GetPassword(TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name));

            //assert
            Assert.Equal(expected, actual);

        }

        [Theory]
        [InlineData("defaultName1", "defaultName1", new[] { "password:newPassword1", "email:newEmail1" }, "defaultName1 newPassword1 newEmail1")]
        [InlineData("defaultName2", "newName2", new[] { "name:newName2", "email:newEmail2" }, "newName2 defaultPassword2 newEmail2")]
        [InlineData("defaultName3", "fixedName", new[] { "name:fixedName", "password:passfix", "email:fixed@email.com" }, "fixedName passfix fixed@email.com")]
        public void UpdatePasswordSuccess(string name, string newName, string[] args, string expected) {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.pswManager;
            string actual;

            //act
            manager.EditPassword(name, args);
            actual = manager.GetPassword(newName);

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void DeletePasswordSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.pswManager;
            string name = TestsHelper.defaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exist;

            //act
            exist = manager.GetPassword(name) is string;
            manager.DeletePassword(name);

            //assert
            Assert.True(exist);
            Assert.Throws<InvalidCommandException>(() => manager.GetPassword(name));

        }

    }
}
