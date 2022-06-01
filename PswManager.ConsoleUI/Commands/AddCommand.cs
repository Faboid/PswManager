using PswManager.Commands;
using PswManager.Database.Models;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands {

    public class AddCommand : BaseCommand<AddCommandArgs> {

        private readonly IAccountCreator dataCreator;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public AddCommand(IAccountCreator dataCreator) {
            this.dataCreator = dataCreator;
        }

        protected override CommandResult RunLogic(AddCommandArgs obj) {

            var result = dataCreator.CreateAccount(new AccountModel(obj.Name, obj.Password, obj.Email));
            return result.Success switch {
                true => new CommandResult("The account has been created successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(AddCommandArgs obj) {

            var result = await dataCreator.CreateAccountAsync(new AccountModel(obj.Name, obj.Password, obj.Email));
            return result.Success switch {
                true => new CommandResult("The account has been created successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

    }
}
