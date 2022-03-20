using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerTests.Commands.Helper;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    public class GetCommandTests {

        public GetCommandTests() {
            var dbFactory = new MemoryDBHandler(1).SetUpDefaultValues().GetDBFactory();
            getCommand = new GetCommand(dbFactory.GetDataReader(), MockedObjects.GetEmptyCryptoAccount());
        }

        readonly GetCommand getCommand;

        [Fact]
        public void CommandSuccess() {

            //arrange
            CommandResult result;
            var obj = ClassBuilder.Build<GetCommand>(new List<string>() { DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name) });

            //act
            result = getCommand.Run(obj);

            //assert
            Assert.Equal(new DefaultValues(1).values[0], result.QueryReturnValue);

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build<GetCommand>(new List<string> { name })
                };

            string missingNameMessage = ErrorReader.GetRequiredError<GetCommand>("Name");

            yield return NewObj(missingNameMessage, "");
            yield return NewObj(missingNameMessage, null);

            yield return NewObj(ErrorReader.GetError<GetCommand, VerifyAccountExistenceAttribute>("Name"), "fakeAccountName");

        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = getCommand.Validate(args).success;
            result = getCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
