using PswManagerTests.TestsHelpers;
using Xunit;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataDeleter {

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            var dbHandler = new MemoryDBHandler();
            var dataDeleter = dbHandler
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataDeleter();
            string name = dbHandler.defaultValues.GetValue(1, DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = dataDeleter.AccountExist(name);
            dataDeleter.DeleteAccount(name);

            //assert
            Assert.True(exists);
            Assert.False(dataDeleter.AccountExist(name));

        }

        [Fact]
        public void DeleteFailure_NonExistentName() {

            //arrange
            var dataDeleter = new MemoryDBHandler()
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataDeleter();
            string name = "randomInexistentName";
            bool exists;

            //act
            exists = dataDeleter.AccountExist(name);
            var result = dataDeleter.DeleteAccount(name);

            //assert
            Assert.False(exists);
            Assert.False(result.Success);

        }
    }
}
