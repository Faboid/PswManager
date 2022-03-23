using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation.Builders;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.Cryptography;

namespace PswManagerLibrary.Commands {
    public sealed class EditCommand : BaseCommand<EditCommandArgs> {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;

        public EditCommand(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(EditCommandArgs arguments) {

            AccountModel newValues = new();
            newValues.Name = arguments.NewName;
            if(!string.IsNullOrWhiteSpace(arguments.NewPassword)) {
                newValues.Password = cryptoAccount.GetPassCryptoService().Encrypt(arguments.NewPassword);
            }
            if(!string.IsNullOrWhiteSpace(arguments.NewEmail)) {
                newValues.Email = cryptoAccount.GetEmaCryptoService().Encrypt(arguments.NewEmail);
            }

            dataEditor.UpdateAccount(arguments.Name, newValues);

            return new CommandResult("The account has been edited successfully.", true);
        }

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

        protected override AutoValidatorBuilder<EditCommandArgs> AddRules(AutoValidatorBuilder<EditCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataEditor));

    }
}
