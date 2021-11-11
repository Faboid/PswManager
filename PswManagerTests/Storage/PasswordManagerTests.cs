using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PswManagerTests;
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
            var result = manager.GetPassword("defaultName1");

            //assert
            Assert.Equal(expected, result);

        }

        public void UpdatePasswordSuccess() {
            //todo - complete this test


            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.pswManager;
            string name = "defaultName1";
            string oldPassword = "defaultPassword1";
            string newPasword = "newPassword1";

            //act

            //assert

        }

    }
}
