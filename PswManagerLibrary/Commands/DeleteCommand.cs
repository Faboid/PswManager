using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;

namespace PswManagerLibrary.Commands {
    public class DeleteCommand : BaseCommand {

        private readonly IPasswordManager pswManager;
        private readonly IUserInput userInput;

        public DeleteCommand(IPasswordManager pswManager, IUserInput userInput) {
            this.pswManager = pswManager;
            this.userInput = userInput;
        }

        public override string GetSyntax() {
            return "delete [name]";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(1, 1);
            collection.AddAccountShouldExistCondition(pswManager);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var result = userInput.YesOrNo("Are you sure? This account will be deleted forever.");
            if(result == false) { return new CommandResult("The operation has been stopped.", false); }

            pswManager.DeletePassword(arguments[0]);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
