using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class GetPassword {

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

    }
}
