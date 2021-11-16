using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string expected = "defaultName1 defaultPassword1 defaultEmail1";

            //act
            var actual = manager.GetPassword("defaultName1");

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void UpdatePasswordSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.pswManager;
            string name = "defaultName1";
            string newPassword = "password:newPassword1";
            string newEmail = "email:newEmail1";
            string expected = "defaultName1 newPassword1 newEmail1";
            string actual;

            //act
            manager.EditPassword(name, new[] { newPassword, newEmail });
            actual = manager.GetPassword(name);

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
