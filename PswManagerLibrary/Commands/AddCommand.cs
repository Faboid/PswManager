using PswManagerLibrary.Storage;
using PswManagerCommands.Validation;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands;

namespace PswManagerLibrary.Commands {
    public class AddCommand : BaseCommand {

        private readonly IPasswordManager pswManager;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public AddCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {

            collection.AddCommonConditions(3, 3);
            collection.Add(3, (args) => pswManager.AccountExist(args[0]) == false, AccountExistsErrorMessage);

            return collection;
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override CommandResult RunLogic(string[] arguments) {

            pswManager.CreatePassword(arguments[0], arguments[1], arguments[2]);

            return new CommandResult("The account has been created successfully.", true);
        }

    }
}
