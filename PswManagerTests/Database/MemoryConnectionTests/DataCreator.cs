using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using Xunit;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataCreator {

        [Fact]
        public void CreateAccountCorrectly() {

            //arrange
            var dataCreator = new MemoryDBHandler()
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataCreator();
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
            var dbHandler = new MemoryDBHandler();
            var dataCreator = dbHandler
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataCreator();
            AccountModel account = new(dbHandler.defaultValues.GetValue(0, DefaultValues.TypeValue.Name), "password", "email");

            //act
            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);

            //assert
            Assert.True(exist);
            Assert.False(result.Success);

        }
    }
}
