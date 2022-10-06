using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Commands.Validation.Builders;
using PswManager.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PswManager.Extensions;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.UI.Console.Commands.Validation.ValidationTypes;
using PswManager.UI.Console.Commands.ArgsModels;
using PswManager.UI.Console.Inner.Interfaces;

namespace PswManager.UI.Console.Commands;

/// <summary>
/// A command to get all existing accounts.
/// </summary>
public class GetAllCommand : BaseCommand<GetAllCommandArgs> {

    static readonly string[] validKeys = new string[] { "names", "passwords", "emails" };
    public const string InexistentKeyErrorMessage = "Invalid key(s) has been found. Valid keys: names, passwords, emails.";
    public const string DuplicateKeyErrorMessage = "Duplicate keys aren't allowed.";

    private static IEnumerable<string> GetAllAccountsAsStrings(IEnumerable<NamedAccountOption> accounts, ValuesToGet toGet) => accounts.AsParallel().Select(x => x.Match(
            some => Unwrap(some, toGet),
            error => SingleErrorToMessage(error.Name, error.ErrorCode),
            () => $"There has been an error retrieving an unknown account."
        )
    );

    private static IAsyncEnumerable<string> GetAllAccountsAsStringsAsync(IAsyncEnumerable<NamedAccountOption> accounts, ValuesToGet toGet) => accounts.Select(x => x.Match(
            some => Task.Run(() => Unwrap(some, toGet)),
            error => SingleErrorToMessage(error.Name, error.ErrorCode).AsTask(),
            () => $"There has been an error retrieving an unknown account.".AsTask()
        )
    );

    private static CommandResult StringListToResult(IEnumerable<string> accounts)
        => new("The list has been retrieved.", true, string.Join(Environment.NewLine, accounts));
    private static async Task<CommandResult> StringListToResultAsync(IAsyncEnumerable<string> accounts)
        => new("The list has been retrieved.", true, await accounts.JoinStrings(Environment.NewLine).ConfigureAwait(false));
    //todo - consider adding overload for CommandResult to give it the possibility of keeping an IEnumerable.
    //it would allow returning without enumerating everything; which would be quite expensive when dealing with
    //huge datasets

    /// <summary>
    /// A simple model for the possible values to retrieve.
    /// </summary>
    private record ValuesToGet() {
        public ValuesToGet(GetAllCommandArgs args) : this() {
            Names = args.SplitKeys().Contains(validKeys[0]);
            Passwords = args.SplitKeys().Contains(validKeys[1]);
            Emails = args.SplitKeys().Contains(validKeys[2]);

            //if they are all false, the user didn't input any constraint. Therefore, they should all be taken.
            if(!Names && !Passwords && !Emails) {
                Names = true;
                Passwords = true;
                Emails = true;
            }
        }

        public bool Names { get; } = true;
        public bool Passwords { get; } = true;
        public bool Emails { get; } = true;
    }

    readonly IAccountReader dataReader;


    public GetAllCommand(IAccountReader dataReader) {
        this.dataReader = dataReader;
    }

    protected override CommandResult RunLogic(GetAllCommandArgs arguments) {
        var toGet = new ValuesToGet(arguments);

        var enumerable = dataReader.ReadAllAccounts();
        var asStrings = GetAllAccountsAsStrings(enumerable, toGet);
        var asCommandResult = StringListToResult(asStrings);
        return asCommandResult;
    }

    protected override async ValueTask<CommandResult> RunLogicAsync(GetAllCommandArgs args) {
        var toGet = new ValuesToGet(args);

        var enumerable = dataReader.ReadAllAccountsAsync();
        var asStrings = GetAllAccountsAsStringsAsync(enumerable, toGet);
        return await StringListToResultAsync(asStrings);
    }

    public override string GetDescription() {
        return "Gets a list of the requested parameters of all accounts.";
    }

    protected override AutoValidatorBuilder<GetAllCommandArgs> AddRules(AutoValidatorBuilder<GetAllCommandArgs> builder) => builder
        .AddRule<ValidValuesRule>()
        .AddRule<NoDuplicateValuesRule>();

    private static string SingleErrorToMessage(string name, ReaderErrorCode errorCode) => errorCode switch {
        ReaderErrorCode.InvalidName => "Some query has received an invalid name.", //this should never happen, but I'll leave it here just in case
        ReaderErrorCode.UsedElsewhere => $"The account {name} is being used elsewhere, so its values cannot be accessed.",
        ReaderErrorCode.DoesNotExist => $"The account {name} doesn't exist anymore. It might've been deleted.",
        _ => $"There has been an unknown error when trying to get the account {name}."
    };

    private static string Unwrap(IAccountModel result, ValuesToGet toGet) {
        var stringRepresenation = Take(result, toGet);
        return Merge(stringRepresenation);
    }

    private static IEnumerable<string> Take(IAccountModel account, ValuesToGet toGet) {
        if(toGet.Names)
            yield return account.Name;
        if(toGet.Passwords)
            yield return account.Password;
        if(toGet.Emails)
            yield return account.Email;
    }

    private static string Merge(IEnumerable<string> values) => string.Join(' ', values);

}
