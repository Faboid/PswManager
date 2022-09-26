using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Database.Models;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands;
public class GetCommand : BaseCommand<GetCommandArgs> {

    private readonly IAccountReader dataReader;

    public GetCommand(IAccountReader dataReader) {
        this.dataReader = dataReader;
    }

    protected override CommandResult RunLogic(GetCommandArgs arguments) {
        var result = dataReader.ReadAccount(arguments.Name);

        return result.Match(
            some => SuccessfulResult(some),
            error => FailureResult(error),
            () => new("There has been an error.", false));
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(GetCommandArgs args) {
        var result = await dataReader.ReadAccountAsync(args.Name).ConfigureAwait(false);

        return result.Match(
            some => SuccessfulResult(some),
            error => FailureResult(error),
            () => new("There has been an error.", false));
    }

    private static string ErrorToString(ReaderErrorCode errorCode) => errorCode switch {
        ReaderErrorCode.Undefined => "There has been an unknown error.",
        ReaderErrorCode.InvalidName => "The given name is not valid.",
        ReaderErrorCode.UsedElsewhere => "This account is being used elsewhere.",
        ReaderErrorCode.DoesNotExist => "The given name doesn't exist.",
        _ => "There has been an unknown error.",
    };

    private static CommandResult FailureResult(ReaderErrorCode errorMessage)
        => new(ErrorToString(errorMessage), false);
    private static CommandResult SuccessfulResult(AccountModel account)
        => new("The account has been retrieved successfully.", true, $"{account.Name} {account.Password} {account.Email}");

    public override string GetDescription() {
        return "Gets the requested command from the saved ones.";
    }

}
