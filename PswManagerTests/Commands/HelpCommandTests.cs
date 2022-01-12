using Moq;
using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {
    public class HelpCommandTests {

        public HelpCommandTests() {
            Mock<ICommand> _mockedCommand = new();
            Mock<ICommand> _mockedTwoCommand = new();

            string mockedCommandSyntax = "mocked [ismocked]";
            string mockedTwoCommandSyntax = "mockedtwo [ismocked]?";
            string mockedCommandDescription = "This command is being mocked and doesn't have any actual functionality.";
            string mockedTwoCommandDescription = "This command is also being mocked. The only difference with the first is that the argument it requests is optional.";

            _mockedCommand.Setup(x => x.GetSyntax()).Returns(mockedCommandSyntax);
            _mockedTwoCommand.Setup(x => x.GetSyntax()).Returns(mockedTwoCommandSyntax);
            _mockedCommand.Setup(x => x.GetDescription()).Returns(mockedCommandDescription);
            _mockedTwoCommand.Setup(x => x.GetDescription()).Returns(mockedTwoCommandDescription);

            Dictionary<string, ICommand> commands = new();
            commands.Add("mocked", _mockedCommand.Object);
            commands.Add("mockedtwo", _mockedTwoCommand.Object);

            helpCommand = new HelpCommand(commands);
            mockedCommand = _mockedCommand.Object;
            mockedTwoCommand = _mockedTwoCommand.Object;
            dicCommands = commands;
        }

        readonly ICommand mockedCommand;
        readonly ICommand mockedTwoCommand;
        readonly IReadOnlyDictionary<string, ICommand> dicCommands;
        readonly HelpCommand helpCommand;

        [Fact]
        public void GetGenericHelpCorrectly() {

            //arrange
            string expectedToContain = string.Join("  ", dicCommands.Keys);

            //act
            var result = helpCommand.Run(Array.Empty<string>());

            //assert
            Assert.True(result.Success);
            Assert.Contains(expectedToContain, result.QueryReturnValue);

        }

        [Fact]
        public void GetSpecificCommandSyntax() {

            //arrange
            string expectedDescription = mockedCommand.GetDescription();
            string expectedSyntax = mockedCommand.GetSyntax();

            //act
            var result = helpCommand.Run(new string[] { "mOcKed" });

            //assert
            Assert.True(result.Success);
            Assert.Equal(expectedDescription, result.BackMessage);
            Assert.Contains(expectedSyntax, result.QueryReturnValue);

        }

        [Fact]
        public void GetSpecificCommandSyntaxWithOptionalArguments() {

            //arrange
            string expectedDescription = mockedTwoCommand.GetDescription();
            string expectedSyntax = mockedTwoCommand.GetSyntax();

            //act
            var result = helpCommand.Run(new string[] { "mOcKedTwo" });

            //assert
            Assert.True(result.Success);
            Assert.Equal(expectedDescription, result.BackMessage);
            Assert.Contains(expectedSyntax, result.QueryReturnValue);
            Assert.Contains("optional", result.QueryReturnValue);

        }

        [Fact]
        public void GetSyntaxButEmptyDictionary() {

            //arrange
            Dictionary<string, ICommand> commands = new();
            HelpCommand helpCommand = new(commands);
            string expected = "There has been an error: the command list is empty.";

            //act
            var result = helpCommand.Run(Array.Empty<string>());

            //assert
            Assert.False(result.Success);
            Assert.Equal(expected, result.BackMessage);

        }


        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "" } };

            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "mocked", "extravalue" } };

            yield return new object[] { HelpCommand.CommandInexistentErrorMessage, new string[] { "nonexistentcommand" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = helpCommand.Validate(args).success;
            result = helpCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }

}
