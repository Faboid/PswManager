using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands {
    public sealed class EditCommand : BaseCommand<EditCommandArgs> {

        private readonly IAccountEditor accountEditor;


        public EditCommand(IAccountEditor accountEditor) {
            this.accountEditor = accountEditor;
        }

        protected override CommandResult RunLogic(EditCommandArgs arguments) {
            var result = accountEditor.UpdateAccount(arguments.Name, new(arguments.NewName, arguments.NewPassword, arguments.NewEmail));

            return result.Success switch {
                true => new CommandResult("The account has been edited successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(EditCommandArgs arguments) {
            var result = await accountEditor.UpdateAccountAsync(arguments.Name, new(arguments.NewName, arguments.NewPassword, arguments.NewEmail)).ConfigureAwait(false);


            return result.Success switch {
                true => new CommandResult("The account has been edited successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

    }
}
