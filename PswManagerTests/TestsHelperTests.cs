using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests {

    [Collection("TestHelperCollection")]
    public class TestsHelperTests {

        [Theory]
        [InlineData("defaultName1", "defaultPassword1", "defaultEmail1")]
        [InlineData("defaultName2", "defaultPassword2", "defaultEmail2")]
        [InlineData("defaultName3", "defaultPassword3", "defaultEmail3")]
        public void SetUpDefaultCorrectly(string name, string password, string email) {

            //arrange
            var pswManager = TestsHelper.pswManager;
            var list = new[] { name, password, email };

            //act
            TestsHelper.SetUpDefault();
            var res = pswManager.GetPassword(list[0]);

            //assert
            Assert.Equal(String.Join(' ', list), res);

        }

    }
}
