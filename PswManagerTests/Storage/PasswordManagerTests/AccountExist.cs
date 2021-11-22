using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class AccountExist {
    
        [Fact]
        public void AccountShouldExist() {

            //arrange
            TestsHelper.SetUpDefault();
            bool exists;

            //act
            exists = TestsHelper.pswManager.AccountExist(TestsHelper.defaultValues.GetValue(0, DefaultValues.TypeValue.Name));

            //assert
            Assert.True(exists);

        }

        [Fact]
        public void AccountShouldNotExist() {

            //arrange
            TestsHelper.SetUpDefault();
            bool exists;

            //act
            exists = TestsHelper.pswManager.AccountExist("fgghawhgri");

            //arrange
            Assert.False(exists);

        }

    }
}
