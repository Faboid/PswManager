using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Storage.AccountBuilderTests {

    [Collection("TestHelperCollection")]
    public class DeleteOne {

        [Fact]
        public void DeleteOneCorrectly() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = builder.Search(name) is not null;
            builder.DeleteOne(name);

            //assert
            Assert.True(exists);
            Assert.Null(builder.Search(name));

        }

        [Fact]
        public void DeleteFailure_NonExistentName() {

            //arrange
            TestsHelper.SetUpDefault();
            AccountBuilder builder = new AccountBuilder(TestsHelper.Paths);
            string name = "randomInexistentName";
            bool exists;

            //act
            exists = builder.Search(name) is not null;

            //assert
            Assert.False(exists);
            Assert.Throws<InexistentAccountException>(() => builder.DeleteOne(name));

        }

    }
}
