using PswManagerLibrary.Exceptions;
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
            TestsHelper.SetUpDefault();
        }

        readonly IPasswordManager pswManager;
        readonly ICommand addCommand;

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

        public static IEnumerable<object[]> IncorrectNumberArgumentsData() {
            yield return new object[] { new string[] { "validName", "validEmail@emaildomain.com" } };
            yield return new object[] { new string[] { "justAName" } };
            yield return new object[] { Array.Empty<string>() };
            yield return new object[] { new string[] { "firstName", "secondName", "passwordhere", "validEmail@emaildomain.com" } };
        }

        [Theory]
        [MemberData(nameof(IncorrectNumberArgumentsData))]
        public void CommandFailure_IncorrectNumberArguments(string[] args) {

            //arrange
            bool valid;

            //act
            valid = addCommand.Validate(args).success;

            //assert
            Assert.False(valid);
            Assert.Throws<InvalidCommandException>(() => addCommand.Run(args));

        }

        [Theory]
        [InlineData("", "somePassValue", "Someemail@value.com")]
        [InlineData("someNameValue", "", "Someemail@value.com")]
        [InlineData("someNameValue", "somePassValue", "")]
        public void CommandFailure_EmptyValues(string name, string password, string email) {

            //arrange
            var args = new string[] { name, password, email };
            bool valid;

            //act
            valid = addCommand.Validate(args).success;

            //assert
            Assert.False(valid);
            Assert.Throws<InvalidCommandException>(() => addCommand.Run(args));
        } 

    }
}
