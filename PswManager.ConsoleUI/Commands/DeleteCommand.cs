using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core;
using PswManager.Core.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands {
    public class DeleteCommand : BaseCommand<DeleteCommandArgs> {

        IUserInput userInput;
        IAccountDeleter dataDeleter;

        private readonly CommandResult stoppedEarlyResult = new("The operation has been stopped.", false);
        private readonly CommandResult successResult = new("Account deleted successfully.", true);

        public DeleteCommand(IAccountDeleter deleter, IUserInput userInput) {
            this.dataDeleter = deleter;
            this.userInput = userInput;
        }

        protected override CommandResult RunLogic(DeleteCommandArgs args) {
            if(StopEarlyQuestion()) { return stoppedEarlyResult; }

            var result = dataDeleter.DeleteAccount(args.Name);

            return result.Success switch {
                true => successResult,
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(DeleteCommandArgs args) {
            if(StopEarlyQuestion()) { return stoppedEarlyResult; }

            var cnnResult = await dataDeleter.DeleteAccountAsync(args.Name).ConfigureAwait(false);

            return cnnResult.Success switch {
                true => successResult,
                false => new CommandResult($"There has been an error: {cnnResult.ErrorMessage}", false)
            };
        }

        public override string GetDescription() {
            return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
        }

        private bool StopEarlyQuestion() {
            //if the user inputs yes, they are sure and want to keep going
            //if the user inputs no, they want to stop
            //as this methods checks if they want to stop, it must be inverted.
            return !userInput.YesOrNo("Are you sure? This account will be deleted forever.");
        }

    }
}
