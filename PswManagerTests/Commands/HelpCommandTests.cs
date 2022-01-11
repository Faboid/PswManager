using Moq;
using PswManagerCommands;
using PswManagerLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {
    public class HelpCommandTests {

        [Fact]
        public void GetGenericHelpCorrectly() {

            //arrange
            Mock<ICommand> fakeCommand = new();
            Mock<ICommand> fakeCommandWithQuestionMark = new();

            string fakeReturn = "fake [isFake]";
            string questionFakeReturn = "questionFake [isFake]?";

            fakeCommand.Setup(x => x.GetSyntax()).Returns(fakeReturn);
            fakeCommandWithQuestionMark.Setup(x => x.GetSyntax()).Returns(questionFakeReturn);

            Dictionary<string, ICommand> commands = new();
            commands.Add("fake", fakeCommand.Object);
            commands.Add("questionFake", fakeCommandWithQuestionMark.Object);

            string expectedToContain = string.Join("  ", commands.Keys);

            HelpCommand helpCommand = new HelpCommand(commands);

            //act
            var result = helpCommand.Run(Array.Empty<string>());

            //assert
            Assert.Contains(expectedToContain, result.QueryReturnValue);

        }

    }

}
