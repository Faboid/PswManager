using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Config;
using PswManagerDatabase;
using PswManagerTests.TestsHelpers;
using Xunit;
using System.IO;
using System;
using PswManagerDatabase.DataAccess;

namespace PswManagerTests.Database.TextFileConnectionTests {

    [Collection("TestHelperCollection")]
    public class DataDeleter {

        public DataDeleter() : base() {
            IDataConnection dataConnection = new TextFileConnection(TestsHelper.Paths);
            dataDeleter = dataConnection;
            dataHelper = dataConnection;
        }

        readonly IDataDeleter dataDeleter;
        readonly IDataHelper dataHelper;

        [Fact]
        public void DeleteAccountCorrectly() {

            //arrange
            TestsHelper.SetUpDefault();
            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);
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
    }
}
