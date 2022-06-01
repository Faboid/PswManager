using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Commands.Validation.Builders;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Core.Cryptography;
using System;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.ConsoleUI.Commands.Validation.ValidationTypes;

namespace PswManager.ConsoleUI.Commands {
    public class GetCommand : BaseCommand<GetCommandArgs> {

        private readonly IDataReader dataReader;
        private readonly ICryptoAccount cryptoAccount;

        public GetCommand(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(GetCommandArgs arguments) {
            ConnectionResult<AccountModel> result = dataReader.GetAccount(arguments.Name);

            if(!result.Success) {
                return FailureResult(result.ErrorMessage);
            }

            (result.Value.Password, result.Value.Email) = cryptoAccount.Decrypt(result.Value.Password, result.Value.Email);
            return SuccessfulResult(result.Value);
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(GetCommandArgs args) {
            ConnectionResult<AccountModel> result = await dataReader.GetAccountAsync(args.Name).ConfigureAwait(false);

            if(!result.Success) {
                return FailureResult(result.ErrorMessage);
            }

            (result.Value.Password, result.Value.Email) = await Task.Run(() => cryptoAccount.Decrypt(result.Value.Password, result.Value.Email)).ConfigureAwait(false);
            return SuccessfulResult(result.Value);
        }

        private static CommandResult FailureResult(string errorMessage)
            => new(errorMessage, false);
        private static CommandResult SuccessfulResult(AccountModel account)
            => new("The account has been retrieved successfully.", true, $"{account.Name} {account.Password} {account.Email}");

        public override string GetDescription() {
            return "Gets the requested command from the saved ones.";
        }

        protected override AutoValidatorBuilder<GetCommandArgs> AddRules(AutoValidatorBuilder<GetCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataReader));

    }
}
