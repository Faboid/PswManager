using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Factories;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using Xunit;

namespace PswManagerTests.Factories {
    public class PasswordManagerFactoryTests {
    
        [Fact]
        public void CreatePswManagerSuccess() {

            //arrange
            var factory = new PasswordManagerFactory(new CryptoAccountFactory());
            IPasswordManager pswManager;

            //act
            pswManager = factory.CreatePasswordManager(new AutoInput(), TestsHelper.Paths, TestsHelper.pswPassword, TestsHelper.emaPassword);

            //assert
            Assert.IsType<PasswordManager>(pswManager);

        }

        [Fact]
        public void CreatePswManagerFailure_UserSaysNo() {

            //arrange
            var factory = new PasswordManagerFactory(new CryptoAccountFactory());
            AutoInput autoInput = new AutoInput() { YesOrNoReturn = false };

            //act

            //assert
            Assert.Throws<CanceledCommandException>(() => factory.CreatePasswordManager(autoInput, TestsHelper.Paths, TestsHelper.pswPassword, TestsHelper.emaPassword));

        }   
        
    }
}
