using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PswManager.Tests.Commands {
    public class BaseCommandTests {

        readonly MockedChildrenCommand command = new MockedChildrenCommand();

        [Fact]
        public async Task ReturnExpectedValue() {

            //arrange
            MockedArgs args = new MockedArgs();

            //act
            var result = command.Run(args);
            var resultAsync = await command.RunAsync(args).ConfigureAwait(false);

            //assert
            Assert.True(result.Success);
            Assert.Equal(command.Result, result.BackMessage);
            Assert.Equal(command.Result, resultAsync.BackMessage);

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
        public async Task ThrowOnNullArgument() {

            Assert.Throws<ArgumentNullException>(() => command.Validate(null));
            Assert.Throws<ArgumentNullException>(() => command.Run(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await command.RunAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

        }

        [Fact]
        public async Task ThrowOnWrongArgumentType() {

            //arrange
            var args = new MockedArgsTwo();

            Assert.Throws<InvalidCastException>(() => command.Validate(args));
            Assert.Throws<InvalidCastException>(() => command.Run(args));
            await Assert.ThrowsAsync<InvalidCastException>(async () => await command.RunAsync(args).ConfigureAwait(false)).ConfigureAwait(false);

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

        protected override ValueTask<CommandResult> RunLogicAsync(MockedArgs args) {
            return ValueTask.FromResult(RunLogic(args));
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
