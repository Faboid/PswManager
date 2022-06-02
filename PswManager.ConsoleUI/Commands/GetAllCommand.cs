using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Commands.Validation.Builders;
using PswManager.Database.Models;
using PswManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.Validation.ValidationTypes;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core.Inner.Interfaces;
using PswManager.Extensions;

namespace PswManager.ConsoleUI.Commands {
    public class GetAllCommand : BaseCommand<GetAllCommandArgs> {

        static readonly string[] validKeys = new string[] { "names", "passwords", "emails" };
        public const string InexistentKeyErrorMessage = "Invalid key(s) has been found. Valid keys: names, passwords, emails.";
        public const string DuplicateKeyErrorMessage = "Duplicate keys aren't allowed.";

        private static CommandResult GetErrorResult(string error) => new($"There has been an error: {error}", false);
        private CommandResult GetAllAccountsValuesResult(IEnumerable<AccountResult> accounts)
            => new("The list has been retrieved.", true, string.Join(Environment.NewLine, accounts.Select(Unwrap)));
        private async Task<CommandResult> GetAllAccountsValuesResultAsync(IAsyncEnumerable<AccountResult> accounts)
            => new("The list has been retrieved.", true, await accounts.Select(Unwrap).JoinStrings(Environment.NewLine).ConfigureAwait(false));
        private static CommandResult GetStringEnumResult(IEnumerable<string> accounts)
            => new("The list has been retrieved.", true, string.Join(Environment.NewLine, accounts));
        private static async Task<CommandResult> GetStringEnumResultAsync(IAsyncEnumerable<string> accounts)
            => new("The list has been retrieved.", true, await accounts.JoinStrings(Environment.NewLine).ConfigureAwait(false));
        //todo - consider adding overload for CommandResult to give it the possibility of keeping an IEnumerable.
        //it would allow returning without enumerating everything; which would be quite expensive when dealing with
        //huge datasets

        private record ValuesToGet() {
            public ValuesToGet(GetAllCommandArgs args) : this() {
                Names = args.SplitKeys().Contains(validKeys[0]);
                Passwords = args.SplitKeys().Contains(validKeys[1]);
                Emails = args.SplitKeys().Contains(validKeys[2]);
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
            var result = dataReader.ReadAllAccounts();

            if(!result.Success) {
                return GetErrorResult(result.ErrorMessage);
            }

            if(string.IsNullOrWhiteSpace(arguments.Keys)) {
                return GetAllAccountsValuesResult(result.Value);
            }

            var toGet = new ValuesToGet(arguments);
            var accounts = UnwrapAll(result.Value, toGet);

            return GetStringEnumResult(accounts);
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(GetAllCommandArgs args) {
            var result = await dataReader.ReadAllAccountsAsync().ConfigureAwait(false);

            if(!result.Success) {
                return GetErrorResult(result.ErrorMessage);
            }

            if(string.IsNullOrWhiteSpace(args.Keys)) {
                return await GetAllAccountsValuesResultAsync(result.Value);
            }

            var toGet = new ValuesToGet(args);
            var accounts = UnwrapAll(result.Value, toGet);
            return await GetStringEnumResultAsync(accounts);
        }

        public override string GetDescription() {
            return "Gets a list of the requested parameters of all accounts.";
        }

        protected override AutoValidatorBuilder<GetAllCommandArgs> AddRules(AutoValidatorBuilder<GetAllCommandArgs> builder) => builder
            .AddRule<ValidValuesRule>()
            .AddRule<NoDuplicateValuesRule>();

        private IEnumerable<string> UnwrapAll(IEnumerable<AccountResult> accounts, ValuesToGet toGet) {
            return accounts.AsParallel().Select(x => Unwrap(x, toGet));
        }

        //as this is cpu-bound work, there is not much that can be done to make it async beside wrapping it in a Task.Run().
        private IAsyncEnumerable<string> UnwrapAll(IAsyncEnumerable<AccountResult> accounts, ValuesToGet toGet) {
            return accounts.Select(x => Task.Run(() => Unwrap(x, toGet)));
        }

        private string Unwrap(AccountResult account) {
            return Unwrap(account, new());
        }

        private string Unwrap(AccountResult result, ValuesToGet toGet) {
            if(result.Success) {
                var stringRepresenation = Take(result.Value, toGet);
                return Merge(stringRepresenation);
            }

            return $"Error when getting {result.NameAccount}: {result.ErrorMessage ?? "Undefined"}";
        }

        private static IEnumerable<string> Take(AccountModel account, ValuesToGet toGet) {
            if(toGet.Names)
                yield return account.Name;
            if(toGet.Passwords)
                yield return account.Password;
            if(toGet.Emails)
                yield return account.Email;
        }

        private static string Merge(IEnumerable<string> values) => string.Join(' ', values);

    }
}
