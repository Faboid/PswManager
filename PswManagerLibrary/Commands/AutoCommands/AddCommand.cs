using PswManagerCommands.Validation;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;

namespace PswManagerLibrary.Commands.AutoCommands {
    public class AddCommand : AutoCommand<AccountInfo> {

        private readonly IDataCreator dataCreator;
        private readonly ICryptoAccount cryptoAccount;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public AddCommand(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
            this.dataCreator = dataCreator;
            this.cryptoAccount = cryptoAccount;
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {

            collection.AddCommonConditions(3, 3);
            collection.Add(
                new IndexHelper(0, collection.NullIndexCondition, collection.NullOrEmptyArgsIndexCondition, collection.CorrectArgsNumberIndexCondition),
                (args) => dataCreator.AccountExist(args[0]) == false, AccountExistsErrorMessage);

            return collection;
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override CommandResult RunLogic(string[] arguments) {

            (arguments[1], arguments[2]) = cryptoAccount.Encrypt(arguments[1], arguments[2]);
            var account = new AccountModel(arguments[0], arguments[1], arguments[2]);
            dataCreator.CreateAccount(account);

            return new CommandResult("The account has been created successfully.", true);
        }

        protected override AutoValidation<AccountInfo> BuildAutoValidator(AutoValidationBuilder<AccountInfo> builder)
            => builder
                .AddLogic(new VerifyAccountExistenceLogic(dataCreator))
                .Build();

    }
}
