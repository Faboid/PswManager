using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerTests.Commands.Helper;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class GetCommandTests {

        public GetCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            getCommand = new GetCommand(dataFactory.GetDataReader(), TestsHelper.CryptoAccount);
        }

        readonly GetCommand getCommand;

        [Fact]
        public void CommandSuccess() {

            //arrange
            CommandResult result;
            var obj = ClassBuilder.Build<GetCommand>(new List<string>() { TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name) });

            //act
            result = getCommand.Run(obj);

            //assert
            Assert.Equal(TestsHelper.DefaultValues.values[0], result.QueryReturnValue);

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
