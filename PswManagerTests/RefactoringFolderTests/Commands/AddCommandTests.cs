using PswManagerLibrary.RefactoringFolder.Commands;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.RefactoringFolderTests.Commands {

    [Collection("TestHelperCollection")]
    public class AddCommandTests {

        public AddCommandTests() {
            pswManager = TestsHelper.PswManager;
            addCommand = new AddCommand(pswManager);
        }

        IPasswordManager pswManager;
        ICommand addCommand;

        [Theory]
        [InlineData("justSomeName#9839", "random@#[ssword", "random@email.it")]
        [InlineData("xmlnyyx", "ightueghtuy", "this@mail.com")]
        [InlineData("valueNamehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
        public void CommandSuccess(string name, string password, string email) {

            //arrange
            var args = new string[] { name, password, email };
            bool exists;

            //act
            exists = pswManager.AccountExist(name);
            addCommand.Run(args);

            //assert
            Assert.False(exists);
            Assert.True(pswManager.AccountExist(name));

        }

    }
}
