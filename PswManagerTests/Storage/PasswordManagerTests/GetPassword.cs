using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class GetPassword {

        [Fact]
        public void GetSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.PswManager;
            string expected = TestsHelper.DefaultValues.values[1];

            //act
            var actual = manager.GetPassword(TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name));

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void GetFailure_NameInexistent() {

            //arrange
            TestsHelper.SetUpDefault();
            var manager = TestsHelper.PswManager;
            string fakeName = "randomInexistentName";

            //act

            //assert
            Assert.Throws<InexistentAccountException>(() => manager.GetPassword(fakeName));

        }

    }
}
