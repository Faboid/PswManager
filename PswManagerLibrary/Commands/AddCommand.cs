using PswManagerCommands.Validation;
using PswManagerCommands;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;
using PswManagerCommands.AbstractCommands;
using PswManagerLibrary.UIConnection.Attributes;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.ValidationLogic;

namespace PswManagerLibrary.Commands {

    public class AddCommand : BaseCommand<AddCommand.Args> {

        private readonly IDataCreator dataCreator;
        private readonly ICryptoAccount cryptoAccount;
        public const string AccountExistsErrorMessage = "The account you're trying to create exists already.";

        public AddCommand(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
            this.dataCreator = dataCreator;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(Args obj) {

            (obj.Password, obj.Email) = cryptoAccount.Encrypt(obj.Password, obj.Email);
            var account = new AccountModel(obj.Name, obj.Password, obj.Email);
            dataCreator.CreateAccount(account);

            return new CommandResult("The account has been created successfully.", true);
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        protected override AutoValidatorBuilder<Args> AddRules(AutoValidatorBuilder<Args> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataCreator));

        public class Args : ICommandInput {

            [VerifyAccountExistence(false, AccountExistsErrorMessage)]
            [Required]
            [Request("Name", "Insert the name of the account.")]
            public string Name { get; set; }

            [Required]
            [Request("Password", "Insert password.")]
            public string Password { get; set; }

            [Required]
            [Request("Email", "Insert email.")]
            public string Email { get; set; }

        }

    }
}
