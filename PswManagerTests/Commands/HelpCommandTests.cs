using Moq;
using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerTests.Commands.Helper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PswManagerTests.Commands {
    public class HelpCommandTests {

        public HelpCommandTests() {
            Mock<ICommand> _mockedCommand = new();
            Mock<ICommand> _mockedTwoCommand = new();

            string mockedCommandDescription = "This command is being mocked and doesn't have any actual functionality.";

            _mockedCommand.Setup(x => x.GetDescription()).Returns(mockedCommandDescription);

            Dictionary<string, ICommand> commands = new();
            commands.Add("mocked", _mockedCommand.Object);

            helpCommand = new HelpCommand(commands);
            mockedCommand = _mockedCommand.Object;
            dicCommands = commands;
        }

        readonly ICommand mockedCommand;
        readonly IReadOnlyDictionary<string, ICommand> dicCommands;
        readonly HelpCommand helpCommand;

        public static IEnumerable<object[]> GetGenericHelpCorrectlyData() {
            yield return new object[] { new List<string>() { } };
            yield return new object[] { new List<string>() { "" } };
            yield return new object[] { new List<string>() { "   " } };
        }

        [Theory]
        [MemberData(nameof(GetGenericHelpCorrectlyData))]
        public void GetGenericHelpCorrectly(List<string> list) {

            //arrange
            string expectedToContain = string.Join("  ", dicCommands.Keys);
            var obj = ClassBuilder.Build<HelpCommand>(list.ToList());

            //act
            var result = helpCommand.Run(obj);

            //assert
            Assert.True(result.Success);
            Assert.Contains(expectedToContain, result.QueryReturnValue);

        }

        [Fact]
        public void GetSpecificCommandDescription() {

            //arrange
            string expectedDescription = mockedCommand.GetDescription();
            var obj = ClassBuilder.Build<HelpCommand>(new List<string>() { "mOcKed" });

            //act
            var result = helpCommand.Run(obj);

            //assert
            Assert.True(result.Success);
            Assert.Equal(expectedDescription, result.BackMessage);

        }

        [Fact]
        public void GetSyntaxButEmptyDictionary() {

            //arrange
            Dictionary<string, ICommand> commands = new();
            HelpCommand helpCommand = new(commands);
            string expected = "There has been an error: the command list is empty.";
            var obj = ClassBuilder.Build<HelpCommand>(new List<string>());

            //act
            var result = helpCommand.Run(obj);

            //assert
            Assert.False(result.Success);
            Assert.Equal(expected, result.BackMessage);

        }

        [Fact]
        public void Failure_GivenCommandDoesNotExist() {

            //arrange
            var args = ClassBuilder.Build<HelpCommand>(new List<string>() { "nonexistentcommand" });
            string expectedErrorMessage = HelpCommand.CommandInexistentErrorMessage;
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
