using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase;
using PswManagerTests.TestsHelpers;
using Xunit;
using PswManagerTests.Database.SQLConnectionTests.Helpers;
using System;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataDeleter : IDisposable {

        public DataDeleter() : base() {
            dbHandler = new TestDatabaseHandler(db_Name);
            IDataFactory dataFactory = new DataFactory(DatabaseType.Sql, dbHandler.DatabaseName);
            dataDeleter = dataFactory.GetDataDeleter();
        }

        const string db_Name = "DataDeleterTestsDB";
        readonly IDataDeleter dataDeleter;
        readonly TestDatabaseHandler dbHandler;

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            dbHandler.SetUpDefaultValues();
            string name = dbHandler.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
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

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
