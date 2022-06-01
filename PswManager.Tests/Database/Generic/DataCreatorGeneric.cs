using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Tests.TestsHelpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PswManager.Tests.Database.Generic {
    public abstract class DataCreatorGeneric : IDisposable {

        public DataCreatorGeneric(ITestDBHandler dbHandler) {
            dataCreator = dbHandler.GetDBFactory().GetDataCreator();
            this.dbHandler = dbHandler;
            dbHandler.SetUpDefaultValues();
        }

        private readonly IDataCreator dataCreator;
        private readonly ITestDBHandler dbHandler;
        protected static readonly int numValues = 1;

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
        public async Task CreateAccountCorrectlyAsync() {

            //arrange
            var account = new AccountModel("validAccountName", "rigteuwokgteuyh", "here@email.it");

            //act
            bool exist = await dataCreator.AccountExistAsync(account.Name).ConfigureAwait(false);
            var result = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);

            //assert
            Assert.False(exist);
            Assert.True(result.Success);
            Assert.True(await dataCreator.AccountExistAsync(account.Name).ConfigureAwait(false));

        }

        [Fact]
        public async Task CreateAccountFailure_AlreadyExists() {

            //arrange
            var account = new AccountModel(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), "password", "email@here.com");

            //act
            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);
            var resultAsync = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);

            //assert
            Assert.True(exist);
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData(null)]
        public async void CreateAccountFailure_InvalidName(string name) {

            //arrange
            var account = new AccountModel(name, "passhere", "ema@here.com");

            //act
            var result = dataCreator.CreateAccount(account);
            var resultAsync = await dataCreator.CreateAccountAsync(account);

            //assert
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateAccountFailure_InvalidPassword(string password) {

            //arrange
            var account = new AccountModel("veryvalidunusedName", password, "valid@email.com");

            //act
            var result = dataCreator.CreateAccount(account);
            var resultAsync = await dataCreator.CreateAccountAsync(account);

            //assert
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateAccountFailure_InvalidEmail(string email) {

            //arrange
            var account = new AccountModel("validNamenongriurh", "somepass", email);

            //act
            var result = dataCreator.CreateAccount(account);
            var resultAsync = await dataCreator.CreateAccountAsync(account);

            //assert
            Assert.False(result.Success);
            Assert.False(resultAsync.Success);

        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
