using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Database.Models;
using PswManager.Core.Cryptography;
using System;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.Core.Inner.Interfaces;

namespace PswManager.ConsoleUI.Commands {
    public class GetCommand : BaseCommand<GetCommandArgs> {

        private readonly IAccountReader dataReader;

        public GetCommand(IAccountReader dataReader) {
            this.dataReader = dataReader;
        }

        protected override CommandResult RunLogic(GetCommandArgs arguments) {
            var result = dataReader.ReadAccount(arguments.Name);

            if(!result.Success) {
                return FailureResult(result.ErrorMessage);
            }

            return SuccessfulResult(result.Value);
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(GetCommandArgs args) {
            var result = await dataReader.ReadAccountAsync(args.Name).ConfigureAwait(false);

            if(!result.Success) {
                return FailureResult(result.ErrorMessage);
            }

            return SuccessfulResult(result.Value);
        }

        private static CommandResult FailureResult(string errorMessage)
            => new(errorMessage, false);
        private static CommandResult SuccessfulResult(AccountModel account)
            => new("The account has been retrieved successfully.", true, $"{account.Name} {account.Password} {account.Email}");

        public override string GetDescription() {
            return "Gets the requested command from the saved ones.";
        }

    }
}
