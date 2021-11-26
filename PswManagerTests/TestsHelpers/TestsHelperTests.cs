using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.TestsHelpers {

    [Collection("TestHelperCollection")]
    public class TestsHelperTests {

        public static IEnumerable<object[]> DefaultData() {
            foreach(string s in TestsHelper.DefaultValues.values) {
                var values = s.Split(' ');
                yield return values;
            }
        }

        [Theory]
        [MemberData(nameof(DefaultData))]
        public void SetUpDefaultCorrectly(string name, string password, string email) {

            //arrange
            var pswManager = TestsHelper.PswManager;
            var list = new[] { name, password, email };

            //act
            TestsHelper.SetUpDefault();
            var res = pswManager.GetPassword(list[0]);

            //assert
            Assert.Equal(String.Join(' ', list), res);

        }

        //todo - add a test to make sure TestsHelper.Dispose deleted the folder

    }
}
