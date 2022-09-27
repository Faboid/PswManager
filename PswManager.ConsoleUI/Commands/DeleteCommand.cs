using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Utils.Options;

namespace PswManager.ConsoleUI.Commands;
public class DeleteCommand : BaseCommand<DeleteCommandArgs> {

    readonly IUserInput userInput;
    readonly IAccountDeleter accountDeleter;

    private readonly CommandResult stoppedEarlyResult = new("The operation has been stopped.", false);

    public DeleteCommand(IAccountDeleter accountDeleter, IUserInput userInput) {
        this.accountDeleter = accountDeleter;
        this.userInput = userInput;
    }

    protected override CommandResult RunLogic(DeleteCommandArgs args) {
        if(StopEarlyQuestion()) { return stoppedEarlyResult; }
        var option = accountDeleter.DeleteAccount(args.Name);
        return ResponseToResult(option, args.Name);
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(DeleteCommandArgs args) {
        if(StopEarlyQuestion()) { return stoppedEarlyResult; }
        var option = await accountDeleter.DeleteAccountAsync(args.Name).ConfigureAwait(false);
        return ResponseToResult(option, args.Name);
    }

    public override string GetDescription() {
        return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
    }

    private static CommandResult ResponseToResult(DeleterResponseCode response, string name) {
        
        if(response == DeleterResponseCode.Success) {
            return new($"{name} has been deleted successfully.", true);
        }

        var errorMessage = response switch {
            DeleterResponseCode.InvalidName => "The given name cannot be empty.",
            DeleterResponseCode.UsedElsewhere => $"{name} is being used elsewhere.",
            DeleterResponseCode.DoesNotExist => $"{name} does not exist.",
            _ => "Unknown error.",
        };

        return new($"There has been an error: {errorMessage}", false);
    }

    private bool StopEarlyQuestion() {
        //if the user inputs yes, they are sure and want to keep going
        //if the user inputs no, they want to stop
        //as this methods checks if they want to stop, it must be inverted.
        return !userInput.YesOrNo("Are you sure? This account will be deleted forever.");
    }

}
