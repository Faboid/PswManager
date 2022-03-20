using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.TestsHelpers;
using Xunit;
using System;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataDeleter : IDisposable {

        public DataDeleter() : base() {
            dbHandler = new TextDatabaseHandler(dbName, 2).SetUpDefaultValues();
            dataDeleter = dbHandler.GetDBFactory().GetDataDeleter();
            dataHelper = dbHandler.GetDBFactory().GetDataHelper();
        }

        readonly IDataDeleter dataDeleter;
        readonly IDataHelper dataHelper;
        readonly TextDatabaseHandler dbHandler;
        const string dbName = "DataDeleterTestsDB";

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            string name = DefaultValues.StaticGetValue(1, DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = dataHelper.AccountExist(name);
            dataDeleter.DeleteAccount(name);

            //assert
            Assert.True(exists);
            Assert.False(dataHelper.AccountExist(name));

        }

        [Fact]
        public void DeleteFailure_NonExistentName() {

            //arrange
            TestsHelper.SetUpDefault();
            string name = "randomInexistentName";
            bool exists;

            //act
            exists = dataHelper.AccountExist(name);
            var result = dataDeleter.DeleteAccount(name);

            //assert
            Assert.False(exists);
            Assert.False(result.Success);

        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
