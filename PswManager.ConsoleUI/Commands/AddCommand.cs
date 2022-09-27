using PswManager.Commands;
using PswManager.Database.Models;
using PswManager.Commands.AbstractCommands;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands;
public class AddCommand : BaseCommand<AddCommandArgs> {

    private readonly IAccountCreator dataCreator;
    public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

    public AddCommand(IAccountCreator dataCreator) {
        this.dataCreator = dataCreator;
    }

    protected override CommandResult RunLogic(AddCommandArgs obj) {
        var result = dataCreator.CreateAccount(new AccountModel(obj.Name, obj.Password, obj.Email));
        return MatchResult(result, obj.Name);
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(AddCommandArgs obj) {

        var result = await dataCreator.CreateAccountAsync(new AccountModel(obj.Name, obj.Password, obj.Email));
        return MatchResult(result, obj.Name);
    }

    private static CommandResult MatchResult(CreatorResponseCode result, string name) {
        
        if(result == CreatorResponseCode.Success) {
            return new CommandResult("The account has been created successfully.", true);
        }

        return new CommandResult($"There has been an error: {ErrorCodeToMessage(result, name)}", false);
    }

    private static string ErrorCodeToMessage(CreatorResponseCode errorCode, string name) => errorCode switch {
        CreatorResponseCode.InvalidName => "The given name cannot be empty.",
        CreatorResponseCode.MissingPassword => $"The password to create the account {name} cannot be empty.",
        CreatorResponseCode.MissingEmail => $"The email to create the account {name} cannot be empty.",
        CreatorResponseCode.AccountExistsAlready => $"The account {name} exists already.",
        CreatorResponseCode.UsedElsewhere => $"The account {name} is being used elsewhere.",
        _ => $"The account {name}'s creation has failed for an unknown reason.",
    };

    public override string GetDescription() {
        return "This command saves an account that can be later retrieved.";
    }

}
