using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class CreatePasswords {

        [Fact]
        public void CreatePasswordSuccess() {

            //arrange
            var manager = TestsHelper.PswManager;
            string name = "name";
            string password = "psw";
            string email = "ema@il";

            //act
            manager.CreatePassword(name, password, email);

            //assert
            Assert.True(manager.AccountExist(name));

        }

    }
}
