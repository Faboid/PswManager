using PswManagerCommands;
using PswManagerCommands.ConcreteCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using System.Linq;

namespace PswManagerLibrary.Commands {
    public class EditCommand : BaseCommand {
        //todo - implement tests for this command
        private readonly IPasswordManager pswManager;

        public EditCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "edit [name] name:[new name]? password:[new password]? email:[new email]?";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(2, 4);
            collection.AddAccountShouldExistCondition(pswManager);
            //todo - add fake email check

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.EditPassword(arguments[0], arguments.Skip(1).ToArray());

            return new CommandResult("The account has been edited successfully.", true);
        }
    }
}
