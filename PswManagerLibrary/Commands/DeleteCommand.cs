using PswManagerCommands;
using PswManagerCommands.TempLocation;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.UIConnection;

namespace PswManagerLibrary.Commands {
    public class DeleteCommand : BaseCommand<AccountName> {

        private readonly IDataDeleter dataDeleter;
        private readonly IUserInput userInput;

        public DeleteCommand(IDataDeleter pswManager, IUserInput userInput) {
            this.dataDeleter = pswManager;
            this.userInput = userInput;
        }

        public override string GetDescription() {
            return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
        }

        protected override IValidationCollection<AccountName> AddConditions(IValidationCollection<AccountName> collection) {

            collection.Add(0, dataDeleter.AccountExist(collection.GetObject().Name), "The given account doesn't exist.");

            return collection;
        }

        protected override CommandResult RunLogic(AccountName args) {
            var result = userInput.YesOrNo("Are you sure? This account will be deleted forever.");
            if(result == false) { return new CommandResult("The operation has been stopped.", false); }

            dataDeleter.DeleteAccount(args.Name);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
