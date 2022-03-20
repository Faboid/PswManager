using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataCreator : IDisposable {

        public DataCreator() {
            dbHandler = new TextDatabaseHandler(dbName, 1).SetUpDefaultValues();
            dataCreator = dbHandler.GetDBFactory().GetDataCreator();
        }

        readonly TextDatabaseHandler dbHandler;
        readonly IDataCreator dataCreator;
        const string dbName = "DataCreatorTestsDB";

        [Fact]
        public void CreateAccountCorrectly() {

            //arrange
            var account = new AccountModel("newLovelyAccount", "girhwugrrigjth", "eco@email.yo");

            //act
            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);
            
            //assert
            Assert.False(exist);
            Assert.True(result.Success);
            Assert.True(dataCreator.AccountExist(account.Name));

        }

        [Fact]
        public void CreateAccountFailure_AlreadyExists() {

            //arrange
            var account = new AccountModel(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), "password", "email");

            //act
            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);

            //assert
            Assert.True(exist);
            Assert.False(result.Success);

        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
