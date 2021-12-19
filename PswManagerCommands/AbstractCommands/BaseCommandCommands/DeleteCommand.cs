using PswManagerCommands.ConcreteCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Storage;

namespace PswManagerCommands.AbstractCommands.BaseCommandCommands {
    public class DeleteCommand : BaseCommand {

        //todo - implement tests for this command
        private readonly IPasswordManager pswManager;

        public DeleteCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
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
            pswManager.DeletePassword(arguments[0]);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
