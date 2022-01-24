using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;

namespace PswManagerLibrary.Commands {
    public class DeleteCommand : BaseCommand {

        private readonly IDataDeleter dataDeleter;
        private readonly IUserInput userInput;

        public DeleteCommand(IDataDeleter pswManager, IUserInput userInput) {
            this.dataDeleter = pswManager;
            this.userInput = userInput;
        }

        public override string GetDescription() {
            return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
        }

        public override string GetSyntax() {
            return "delete [name]";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(1, 1);
            collection.AddAccountShouldExistCondition(0, dataDeleter);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var result = userInput.YesOrNo("Are you sure? This account will be deleted forever.");
            if(result == false) { return new CommandResult("The operation has been stopped.", false); }

            dataDeleter.DeleteAccount(arguments[0]);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
