using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using System;
using Xunit;

namespace PswManagerTests.Database.Generic {
    public abstract class DataCreatorGeneric : IDisposable {

        public DataCreatorGeneric(ITestDBHandler dbHandler) {
            dataCreator = dbHandler.GetDBFactory().GetDataCreator();
            this.dbHandler = dbHandler;
            dbHandler.SetUpDefaultValues();
        }

        private readonly IDataCreator dataCreator;
        private readonly ITestDBHandler dbHandler;

        [Fact]
        public void CreateAccountCorrectly() {

            //arrange
            var account = new AccountModel("newLovelyAccount", "ighreiibnivetngi", "this@email.com");

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
            var account = new AccountModel(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), "password", "email@here.com");

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
