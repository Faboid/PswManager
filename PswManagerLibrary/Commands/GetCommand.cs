using PswManagerCommands;
using PswManagerCommands.ConcreteCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.Commands {
    public class GetCommand : BaseCommand {

        private readonly IPasswordManager pswManager;

        public GetCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "get [name]";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {

            collection.AddCommonConditions(1, 1);
            collection.AddAccountShouldExistCondition(pswManager);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var value = pswManager.GetPassword(arguments[0]);

            return new CommandResult("The account has been retrieved successfully.", true, value);
        }
    }
}
