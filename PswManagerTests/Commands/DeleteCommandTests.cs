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

    public class DeleteCommandTests {

        public DeleteCommandTests() {
            var dbFactory = new MemoryDBHandler(1).SetUpDefaultValues().GetDBFactory();
            delCommand = new DeleteCommand(dbFactory.GetDataDeleter(), MockedObjects.GetDefaultAutoInput());
            dataHelper = dbFactory.GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly DeleteCommand delCommand;

        [Fact]
        public void DeleteSuccessfully() {

            //arrange
            string name = DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name);
            var obj = ClassBuilder.Build<DeleteCommand>(new List<string> { name });

            //act
            bool exist = dataHelper.AccountExist(name);
            delCommand.Run(obj);

            //assert
            Assert.True(exist);
            Assert.False(dataHelper.AccountExist(name));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build<DeleteCommand>(new List<string> { name })
                };

            string missingNameMessage = ErrorReader.GetRequiredError<DeleteCommand>("Name");

            yield return NewObj(missingNameMessage, null);
            yield return NewObj(missingNameMessage, "");

            yield return NewObj(ErrorReader.GetError<DeleteCommand, VerifyAccountExistenceAttribute>("Name"), "fakeAccountName");

        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = delCommand.Validate(args).success;
            result = delCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
