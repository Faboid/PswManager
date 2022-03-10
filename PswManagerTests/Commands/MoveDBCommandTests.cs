using Moq;
using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.Config;
using PswManagerLibrary.Commands;
using PswManagerTests.Commands.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {
    public class MoveDBCommandTests {

        readonly MoveDatabaseCommand moveDBCommand;

        public MoveDBCommandTests() {
            Mock<IPaths> paths = new();
            moveDBCommand = new MoveDatabaseCommand(paths.Object);
        }

        [Fact]
        public void MoveSuccessfully() {

            //todo - since I'm planning to change IPaths's returns from void to ConnectionResult,
            //it's not yet possible to properly mock its methods.
            //Hence, I'll implement this test only afterward
            

            //arrange

            //act

            //assert

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string path)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build<MoveDatabaseCommand>(new List<string> { path })
                };

            yield return NewObj(MoveDatabaseCommand.InexistentDirectoryErrorMessage, "NotAValidPath");

            yield return NewObj(ErrorReader.GetRequiredError<MoveDatabaseCommand>("Path"), null);
            yield return NewObj(ErrorReader.GetRequiredError<MoveDatabaseCommand>("Path"), "");

        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = moveDBCommand.Validate(args).success;
            result = moveDBCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
