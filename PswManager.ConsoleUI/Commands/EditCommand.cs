using PswManager.Commands;
using PswManager.Commands.AbstractCommands;
using PswManager.Commands.Validation.Builders;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Core.Cryptography;
using System.Threading.Tasks;
using PswManager.ConsoleUI.Commands.ArgsModels;
using PswManager.ConsoleUI.Commands.Validation.ValidationTypes;

namespace PswManager.ConsoleUI.Commands {
    public sealed class EditCommand : BaseCommand<EditCommandArgs> {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;

        public EditCommand(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(EditCommandArgs arguments) {
            AccountModel newValues = ArgsToEncryptedModel(arguments);
            var result = dataEditor.UpdateAccount(arguments.Name, newValues);

            return result.Success switch {
                true => new CommandResult("The account has been edited successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(EditCommandArgs args) {
            AccountModel newValues = await Task.Run(() => ArgsToEncryptedModel(args)).ConfigureAwait(false);
            var result = await dataEditor.UpdateAccountAsync(args.Name, newValues).ConfigureAwait(false);

            return result.Success switch {
                true => new CommandResult("The account has been edited successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        private AccountModel ArgsToEncryptedModel(EditCommandArgs args) {
            AccountModel newValues = new();
            newValues.Name = args.NewName;
            if(!string.IsNullOrWhiteSpace(args.NewPassword)) {
                newValues.Password = cryptoAccount.GetPassCryptoService().Encrypt(args.NewPassword);
            }
            if(!string.IsNullOrWhiteSpace(args.NewEmail)) {
                newValues.Email = cryptoAccount.GetEmaCryptoService().Encrypt(args.NewEmail);
            }

            return newValues;
        }

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

        protected override AutoValidatorBuilder<EditCommandArgs> AddRules(AutoValidatorBuilder<EditCommandArgs> builder) => builder
            .AddRule(new VerifyAccountExistenceRule(dataEditor));

    }
}
