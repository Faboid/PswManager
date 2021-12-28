using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Storage.AccountBuilderTests {

    [Collection("TestHelperCollection")]
    public class Search {

        [Fact]
        public void SearchShouldFind() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            int? actual;
            int expected = 2;

            //act
            actual = builder.Search(TestsHelper.DefaultValues.GetValue(2, DefaultValues.TypeValue.Name));

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void SearchInexistentGetNull() {

            //arrange
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            int? actual;
            int? expected = null;

            //act
            actual = builder.Search("thisnamedoesn'texist");

            //assert
            Assert.Equal(expected, actual);

        }

    }
}
