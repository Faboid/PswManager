using PswManagerLibrary.Exceptions;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Storage.PasswordManagerTests {

    [Collection("TestHelperCollection")]
    public class CreatePasswords {

        [Fact]
        public void CreatePasswordSuccess() {

            //arrange
            var manager = TestsHelper.PswManager;
            string name = "name";
            string password = "psw";
            string email = "ema@il";

            //act
            manager.CreatePassword(name, password, email);

            //assert
            Assert.True(manager.AccountExist(name));

        }

        [Fact]
        public void CreatePasswordFailure_NameExists() {

            //arrange
            var manager = TestsHelper.PswManager;
            string name = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);
            string password = "somepass";
            string email = "someemail";
            bool exists;
            string errorMessage = "The account you're trying to create exists already.";

            //act
            var exception = Record.Exception(() => manager.CreatePassword(name, password, email));
            exists = manager.AccountExist(name);

            //assert
            Assert.True(exists);
            Assert.IsType<AccountExistsAlreadyException>(exception);
            Assert.Equal(errorMessage, exception.Message);

        }

    }
}
