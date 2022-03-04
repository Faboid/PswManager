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

            //string validPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "FillerFolder");

            //yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "" } };

            yield return NewObj(MoveDatabaseCommand.InexistentDirectoryErrorMessage, "NotAValidPath");
            yield return NewObj(MoveDatabaseCommand.InexistentDirectoryErrorMessage, "");

            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validPath, "eiwghrywhgi" } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validPath, "tirhtewygh", "email@somewhere.com" } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validPath, "tirhtewygh", "email@somewhere.com", "somevalue" } };
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
