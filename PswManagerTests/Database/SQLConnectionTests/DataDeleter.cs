using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase;
using PswManagerTests.TestsHelpers;
using Xunit;
using PswManagerTests.Database.SQLConnectionTests.Helpers;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataDeleter {

        public DataDeleter() : base() {
            dbHandler = new TestDatabaseHandler(db_Name);
            IDataFactory dataFactory = new DataFactory(dbHandler.DatabaseName);
            dataDeleter = dataFactory.GetDataDeleter();
        }

        const string db_Name = "DataDeleterTestsDB";
        readonly IDataDeleter dataDeleter;
        readonly TestDatabaseHandler dbHandler;

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            dbHandler.SetUpDefaultValues();
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
            dbHandler.SetUpDefaultValues();
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
