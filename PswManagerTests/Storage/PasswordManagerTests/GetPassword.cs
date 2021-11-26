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
            var manager = TestsHelper.PswManager;
            string expected = TestsHelper.DefaultValues.values[1];

            //act
            var actual = manager.GetPassword(TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name));

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
