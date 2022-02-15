using PswManagerCommands.Validation;
using PswManagerCommands;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using PswManagerCommands.Parsing.Attributes;
using PswManagerCommands.Validation.Attributes;
using System;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;
using PswManagerCommands.TempLocation;

namespace PswManagerLibrary.Commands {
    public class AddCommand : BaseCommand<AccountInfo> {

        private readonly IDataCreator dataCreator;
        private readonly ICryptoAccount cryptoAccount;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public class CommandArguments : ICommandInput {
            [ParseableKey("name")] public string Name { get; set; }
            [ParseableKey("pass")] public string Password { get; set; }
            [ParseableKey("ema")] public string Email{ get; set; }
        }

        public AddCommand(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
            this.dataCreator = dataCreator;
            this.cryptoAccount = cryptoAccount;
        }

        protected override IValidationCollection<AccountInfo> AddConditions(IValidationCollection<AccountInfo> collection) {
            //todo - introduce conditions on whether the parameters are assigned
            collection.Add(0, dataCreator.AccountExist(collection.GetObject().Name) == false, AccountExistsErrorMessage);
          
            return collection;
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override CommandResult RunLogic(AccountInfo obj) {

            (obj.Password, obj.Email) = cryptoAccount.Encrypt(obj.Password, obj.Email);
            var account = new AccountModel(obj.Name, obj.Password, obj.Email);
            dataCreator.CreateAccount(account);

            return new CommandResult("The account has been created successfully.", true);
        }

    }
}
