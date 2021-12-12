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

        [Fact]
        public void CommandFailure_AccountExistsAlready() {

            //arrange
            TestsHelper.SetUpDefault();
            var args = new string[] { TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name), "randompass", "randomemail" };

            //act & assert
            Failure_DefaultTestType_ThrowInvalidCommandException(args);

        }

        public static IEnumerable<object[]> NullArgumentsData() {
            yield return new object[] { null };
            yield return new object[] { new string[] { null, "&&%£@#[][+*é", "valueNameHere@thisdomain.com" } };
            yield return new object[] { new string[] { "valuenare", null, "valueNameHere@thisdomain.com" } };
            yield return new object[] { new string[] { "justvalue##", "riewhguyrui", null } };
        }

        [Theory]
        [MemberData(nameof(NullArgumentsData))]
        public void CommandFailure_NullArguments(string[] args) {

            Failure_DefaultTestType_ThrowInvalidCommandException(args);

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

            //act & assert
            Failure_DefaultTestType_ThrowInvalidCommandException(args);

        }

        [Theory]
        [InlineData("", "somePassValue", "Someemail@value.com")]
        [InlineData("someNameValue", "", "Someemail@value.com")]
        [InlineData("someNameValue", "somePassValue", "")]
        public void CommandFailure_EmptyValues(string name, string password, string email) {

            //arrange
            var args = new string[] { name, password, email };

            //act & assert
            Failure_DefaultTestType_ThrowInvalidCommandException(args);
        } 

        private void Failure_DefaultTestType_ThrowInvalidCommandException(string[] args) {

            //arrange
            bool valid;

            //act
            valid = addCommand.Validate(args).success;

            //assert
            Assert.False(valid);
            Assert.Throws<InvalidCommandException>(() => addCommand.Run(args));

        }

    }
}
