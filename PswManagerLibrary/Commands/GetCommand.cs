using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Extensions;
using System;

namespace PswManagerLibrary.Commands {
    public class GetCommand : BaseCommand<GetCommandArgs> {

        private readonly IDataReader dataReader;
        private readonly ICryptoAccount cryptoAccount;

        public GetCommand(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(GetCommandArgs arguments) {
            ConnectionResult<AccountModel> result = dataReader.GetAccount(arguments.Name);

            if(result.Success) {
                (result.Value.Password, result.Value.Email) = cryptoAccount.Decrypt(result.Value.Password, result.Value.Email);
                string outputVal = $"{result.Value.Name} {result.Value.Password} {result.Value.Email}";
                return new CommandResult("The account has been retrieved successfully.", true, outputVal);
            } else {
                return new CommandResult(result.ErrorMessage, false);
            }
        }

        public override string GetDescription() {
            return "Gets the requested command from the saved ones.";
        }

        protected override AutoValidatorBuilder<GetCommandArgs> AddRules(AutoValidatorBuilder<GetCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataReader));

    }
}
