using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Database.Models;

namespace PswManager.ConsoleUI.Commands;

/// <summary>
/// Command used to edit an account.
/// </summary>
public sealed class EditCommand : BaseCommand<EditCommandArgs> {

    private readonly IAccountEditor accountEditor;


    public EditCommand(IAccountEditor accountEditor) {
        this.accountEditor = accountEditor;
    }

    protected override CommandResult RunLogic(EditCommandArgs arguments) {
        var result = accountEditor.UpdateAccount(arguments.Name, new AccountModel(arguments.NewName, arguments.NewPassword, arguments.NewEmail));
        return ToCommandResult(result, arguments);
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(EditCommandArgs arguments) {
        var result = await accountEditor.UpdateAccountAsync(arguments.Name, new AccountModel(arguments.NewName, arguments.NewPassword, arguments.NewEmail)).ConfigureAwait(false);
        return ToCommandResult(result, arguments);
    }

    private static CommandResult ToCommandResult(EditorResponseCode result, EditCommandArgs arguments) {
        if(EditorResponseCode.Success == result) {
            return new("The account has been edited successfully.", true);
        }

        return new(ToErrorMessage(result, arguments.Name, arguments.NewName), false);
    }

    private static string ToErrorMessage(EditorResponseCode errorCode, string name, string newName) => "There has been an error: " + errorCode switch {
        EditorResponseCode.InvalidName => "The given name cannot be empty.",
        EditorResponseCode.UsedElsewhere => $"{name} is being used elsewhere.",
        EditorResponseCode.DoesNotExist => $"{name} does not exist.",
        EditorResponseCode.NewNameUsedElsewhere => $"{newName} is being used elsewhere.",
        EditorResponseCode.NewNameExistsAlready => $"{newName} exists already. As the name is occupied, it's not possible to update {name} to it.",
        _ => "An unknown error has occurred.",
    };

    public override string GetDescription() {
        return "Edits an existing account with the provided arguments.";
    }

}
