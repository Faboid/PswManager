using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation.Builders;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.UIConnection;
using PswManagerLibrary.UIConnection.Attributes;

namespace PswManagerLibrary.Commands {
    public class DeleteCommand : BaseCommand<DeleteCommandArgs> {

        private readonly IDataDeleter dataDeleter;
        private readonly IUserInput userInput;

        public DeleteCommand(IDataDeleter pswManager, IUserInput userInput) {
            this.dataDeleter = pswManager;
            this.userInput = userInput;
        }

        protected override CommandResult RunLogic(DeleteCommandArgs args) {
            var result = userInput.YesOrNo("Are you sure? This account will be deleted forever.");
            if(result == false) { return new CommandResult("The operation has been stopped.", false); }

            var cnnResult = dataDeleter.DeleteAccount(args.Name);

            return cnnResult.Success switch {
                true => new CommandResult("Account deleted successfully.", true),
                false => new CommandResult($"There has been an error: {cnnResult.ErrorMessage}", false)
            };
        }

        public override string GetDescription() {
            return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
        }

        protected override AutoValidatorBuilder<DeleteCommandArgs> AddRules(AutoValidatorBuilder<DeleteCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataDeleter));

    }
}
