using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner.Interfaces;

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
        return OptionToResult(option, args.Name);
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(DeleteCommandArgs args) {
        if(StopEarlyQuestion()) { return stoppedEarlyResult; }
        var option = await accountDeleter.DeleteAccountAsync(args.Name).ConfigureAwait(false);
        return OptionToResult(option, args.Name);
    }

    public override string GetDescription() {
        return "This command deletes an account. Note that the deletion is final: it won't be possible to go back.";
    }

    private CommandResult OptionToResult(Option<DeleterErrorCode> option, string name) => option.Bind<string>(x => x switch {
            DeleterErrorCode.InvalidName => "The given name cannot be empty.",
            DeleterErrorCode.UsedElsewhere => $"{name} is being used elsewhere.",
            DeleterErrorCode.DoesNotExist => $"{name} does not exist.",
            _ => "Unknown error.",
        })
        .Match<CommandResult>(some => new($"There has been an error: {some}", false), () => new($"{name} has been deleted successfully.", true)
    );

    private bool StopEarlyQuestion() {
        //if the user inputs yes, they are sure and want to keep going
        //if the user inputs no, they want to stop
        //as this methods checks if they want to stop, it must be inverted.
        return !userInput.YesOrNo("Are you sure? This account will be deleted forever.");
    }

}
