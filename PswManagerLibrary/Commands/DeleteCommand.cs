using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation.Builders;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.UIConnection;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands {
    public class DeleteCommand : BaseCommand<DeleteCommandArgs> {

        private readonly IDataDeleter dataDeleter;
        private readonly IUserInput userInput;

        private readonly CommandResult stoppedEarlyResult = new("The operation has been stopped.", false);
        private readonly CommandResult successResult = new("Account deleted successfully.", true);

        public DeleteCommand(IDataDeleter dataDeleter, IUserInput userInput) {
            this.dataDeleter = dataDeleter;
            this.userInput = userInput;
        }

        protected override CommandResult RunLogic(DeleteCommandArgs args) {
            if(StopEarlyQuestion()) { return stoppedEarlyResult; }

            var cnnResult = dataDeleter.DeleteAccount(args.Name);

            return cnnResult.Success switch {
                true => successResult,
                false => new CommandResult($"There has been an error: {cnnResult.ErrorMessage}", false)
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

        protected override AutoValidatorBuilder<DeleteCommandArgs> AddRules(AutoValidatorBuilder<DeleteCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataDeleter));

        private bool StopEarlyQuestion() {
            //if the user inputs yes, they are sure and want to keep going
            //if the user inputs no, they want to stop
            //as this methods checks if they want to stop, it must be inverted.
            return !userInput.YesOrNo("Are you sure? This account will be deleted forever.");
        }

    }
}
