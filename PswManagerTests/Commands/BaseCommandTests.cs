using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using System;
using Xunit;

namespace PswManagerTests.Commands {
    public class BaseCommandTests {

        readonly MockedChildrenCommand command = new MockedChildrenCommand();

        [Fact]
        public void ReturnExpectedValue() {

            //arrange
            MockedArgs args = new MockedArgs();

            //act
            var result = command.Run(args);

            //assert
            Assert.True(result.Success);
            Assert.Equal(command.Result, result.BackMessage);

        }

        [Fact]
        public void GetCorrectGenericArgumentsType() {

            //arrange
            Type expected = typeof(MockedArgs);

            //act
            var actual = command.GetCommandInputType;

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void ThrowOnNullArgument() {

            Assert.Throws<ArgumentNullException>(() => command.Validate(null));
            Assert.Throws<ArgumentNullException>(() => command.Run(null));

        }

        [Fact]
        public void ThrowOnWrongArgumentType() {

            //arrange
            var args = new MockedArgsTwo();

            Assert.Throws<InvalidCastException>(() => command.Validate(args));
            Assert.Throws<InvalidCastException>(() => command.Run(args));

        }

    }

    public class MockedChildrenCommand : BaseCommand<MockedArgs> {

        public readonly string Result = "Success!";

        public override string GetDescription() {
            return "A fake command for testing.";
        }

        protected override CommandResult RunLogic(MockedArgs obj) {
            return new CommandResult(Result, true);
        }
    }

    public class MockedArgs : ICommandInput {

        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

    }

    public class MockedArgsTwo : ICommandInput {
        public string Name { get; set; }

    }

}
