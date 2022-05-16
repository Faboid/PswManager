using PswManager.Database.DataAccess.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PswManager.Tests.Database.Generic {
    public abstract class DataDeleterGeneric : IDisposable {

        public DataDeleterGeneric(ITestDBHandler dbHandler) {
            this.dbHandler = dbHandler.SetUpDefaultValues();
            dataDeleter = dbHandler.GetDBFactory().GetDataDeleter();
        }

        readonly IDataDeleter dataDeleter;
        readonly ITestDBHandler dbHandler;
        static protected readonly int numValues = 1;

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            string name = dbHandler.GetDefaultValues().GetValue(0, TestsHelpers.DefaultValues.TypeValue.Name);
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
        public async Task DeleteAccountCorrectlyAsynchronously() {

            //arrange
            string name = dbHandler.GetDefaultValues().GetValue(0, TestsHelpers.DefaultValues.TypeValue.Name);
            bool exists;

            //act
            exists = await dataDeleter.AccountExistAsync(name).ConfigureAwait(false);
            var result = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

            //assert
            Assert.True(exists);
            Assert.True(result.Success);
            Assert.False(await dataDeleter.AccountExistAsync(name).ConfigureAwait(false));

        }

        [Fact]
        public async Task DeleteFailure_NonExistentName() {

            //arrange
            string name = "gerobhipubihtsiyhrti";
            bool exists;

            //act
            exists = dataDeleter.AccountExist(name);
            var result = dataDeleter.DeleteAccount(name);
            var resultAsync = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

            //assert
            Assert.False(exists);
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
