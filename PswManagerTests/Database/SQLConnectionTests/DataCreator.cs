using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using Xunit;
using System;
using PswManagerTests.Database.SQLConnectionTests.Helpers;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataCreator : IDisposable {

        public DataCreator() {
            dbHandler = new(db_Name);
            IDataFactory dataFactory = new DataFactory(dbHandler.DatabaseName);
            dataCreator = dataFactory.GetDataCreator();
        }

        const string db_Name = "DataCreatorTestsDB";
        readonly IDataCreator dataCreator;
        readonly TestDatabaseHandler dbHandler;

        [Fact]
        public void CreateAccountCorrectly() {

            //arrange
            dbHandler.SetUpDefaultValues();
            AccountModel account = new("newLovelyAccount", "girhwugrrigjth", "eco@email.yo");

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
            dbHandler.SetUpDefaultValues();
            AccountModel account = new(dbHandler.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name), "password", "email");

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
