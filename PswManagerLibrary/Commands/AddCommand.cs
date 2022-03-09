using PswManagerCommands.Validation;
using PswManagerCommands;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using PswManagerCommands.AbstractCommands;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.Commands.ArgsModels;

namespace PswManagerLibrary.Commands {

    public class AddCommand : BaseCommand<AddCommandArgs> {

        private readonly IDataCreator dataCreator;
        private readonly ICryptoAccount cryptoAccount;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public AddCommand(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
            this.dataCreator = dataCreator;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(AddCommandArgs obj) {

            (obj.Password, obj.Email) = cryptoAccount.Encrypt(obj.Password, obj.Email);
            var account = new AccountModel(obj.Name, obj.Password, obj.Email);
            dataCreator.CreateAccount(account);

            return new CommandResult("The account has been created successfully.", true);
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        protected override AutoValidatorBuilder<AddCommandArgs> AddRules(AutoValidatorBuilder<AddCommandArgs> builder) => builder
            .AddRule<VerifyAccountExistenceRule>(dataCreator);
        
    }
}
