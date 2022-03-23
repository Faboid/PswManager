using PswManagerCommands;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerTests.Commands.Helper;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    public class AddCommandTests {

        public AddCommandTests() {
            var dbFactory = new MemoryDBHandler(1).SetUpDefaultValues().GetDBFactory();
            addCommand = new AddCommand(dbFactory.GetDataCreator(), MockedObjects.GetEmptyCryptoAccount());
            dataHelper = dbFactory.GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly ICommand addCommand;

        [Theory]
        [InlineData("justSomeName#9839", "random@#[ssword", "random@email.it")]
        [InlineData("xmlnyyx", "ightueghtuy", "this@mail.com")]
        [InlineData("valueNamehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
        public void AddSuccessfully(string name, string password, string email) {

            //arrange
            var obj = ClassBuilder.Build<AddCommand>(new List<string> { password, name, email});
            bool exists;
            CommandResult result;

            //act
            exists = dataHelper.AccountExist(name);
            result = addCommand.Run(obj);

            //assert
            Assert.False(exists);
            Assert.True(result.Success);
            Assert.True(dataHelper.AccountExist(name));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name, string password, string email) 
                => new object[] { 
                    errorMessage, 
                    ClassBuilder.Build<AddCommand>(new List<string> { password, name, email}) 
                };

            string existingName = DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name);
            string validName = "someRandomNonexistentAccountName";

            //check for empty/null values
            yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Name"), "", null, "email@here.com");
            yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Name"), null, "somepass", "email@here.com");
            yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Password"), validName, null, "email@here.com");
            yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Email"), validName, "rightuewih", "");

            yield return NewObj(ErrorReader.GetError<AddCommand, VerifyAccountExistenceAttribute>("Name"), existingName, "somevalidPassword", "someValidEmail@email.com");
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = addCommand.Validate(args).success;
            result = addCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
