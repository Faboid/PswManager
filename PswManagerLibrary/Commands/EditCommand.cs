using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Extensions;

namespace PswManagerLibrary.Commands {
    public sealed class EditCommand : BaseCommand<EditAccountModel> {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;

        public EditCommand(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

        protected override IValidationCollection<EditAccountModel> AddConditions(IValidationCollection<EditAccountModel> collection) {

            //todo - check if at least one of the "new" properties have a value
            collection.AddAccountShouldExistCondition(0, collection.GetObject().Name, dataEditor);

            return collection;
        }

        protected override CommandResult RunLogic(EditAccountModel arguments) {

            AccountModel newValues = new();
            newValues.Name = arguments.NewName;
            if(!string.IsNullOrWhiteSpace(arguments.NewPassword)) {
                newValues.Password = cryptoAccount.GetPassCryptoString().Encrypt(arguments.NewPassword);
            }
            if(!string.IsNullOrWhiteSpace(arguments.NewEmail)) {
                newValues.Email = cryptoAccount.GetEmaCryptoString().Encrypt(arguments.NewEmail);
            }

            dataEditor.UpdateAccount(arguments.Name, newValues);

            return new CommandResult("The account has been edited successfully.", true);
        }

    }
}
