using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;

namespace PswManager.ConsoleUI.Commands {
    public sealed class EditCommand : BaseCommand<EditCommandArgs> {

        private readonly IAccountEditor accountEditor;


        public EditCommand(IAccountEditor accountEditor) {
            this.accountEditor = accountEditor;
        }

        protected override CommandResult RunLogic(EditCommandArgs arguments) {
            var result = accountEditor.UpdateAccount(arguments.Name, new(arguments.NewName, arguments.NewPassword, arguments.NewEmail));
            return result.Match<CommandResult>(some => new(ToErrorMessage(some, arguments.Name, arguments.NewName), false), () => new("The account has been edited successfully.", true));
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(EditCommandArgs arguments) {
            var result = await accountEditor.UpdateAccountAsync(arguments.Name, new(arguments.NewName, arguments.NewPassword, arguments.NewEmail)).ConfigureAwait(false);
            return result.Match<CommandResult>(some => new(ToErrorMessage(some, arguments.Name, arguments.NewName), false), () => new("The account has been edited successfully.", true));
        }

        public static string ToErrorMessage(EditorErrorCode errorCode, string name, string newName) => "There has been an error: " + errorCode switch {
            EditorErrorCode.InvalidName => "The given name cannot be empty.",
            EditorErrorCode.UsedElsewhere => $"{name} is being used elsewhere.",
            EditorErrorCode.DoesNotExist => $"{name} does not exist.",
            EditorErrorCode.NewNameUsedElsewhere => $"{newName} is being used elsewhere.",
            EditorErrorCode.NewNameExistsAlready => $"{newName} exists already. As the name is occupied, it's not possible to update {name} to it.",
            _ => "An unknown error has occurred.",
        };

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

    }
}
