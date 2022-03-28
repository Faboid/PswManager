using PswManagerDatabase.DataAccess.Interfaces;
using System;
using Xunit;

namespace PswManagerTests.Database.Generic {
    public abstract class DataDeleterGeneric : IDisposable {

        public DataDeleterGeneric(ITestDBHandler dbHandler) {
            this.dbHandler = dbHandler.SetUpDefaultValues();
            dataDeleter = dbHandler.GetDBFactory().GetDataDeleter();
        }

        readonly IDataDeleter dataDeleter;
        readonly ITestDBHandler dbHandler;

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            string name = dbHandler.GetDefaultValues().GetValue(1, TestsHelpers.DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = dataDeleter.AccountExist(name);
            var result = dataDeleter.DeleteAccount(name);

            //assert
            Assert.True(exists);
            Assert.True(result.Success);
            Assert.False(dataDeleter.AccountExist(name));
        
        }

        [Fact]
        public void DeleteFailure_NonExistentName() {

            //arrange
            string name = "gerobhipubihtsiyhrti";
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
