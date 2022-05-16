using PswManager.Commands;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Core.Cryptography;
using PswManager.Commands.AbstractCommands;
using PswManager.Core.Commands.Validation.ValidationLogic;
using PswManager.Core.Commands.ArgsModels;
using PswManager.Commands.Validation.Builders;
using System.Threading.Tasks;

namespace PswManager.Core.Commands {

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
            var result = dataCreator.CreateAccount(account);

            return result.Success switch {
                true => new CommandResult("The account has been created successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        protected override async ValueTask<CommandResult> RunLogicAsync(AddCommandArgs obj) {

            (obj.Password, obj.Email) = await Task.Run(() => cryptoAccount.Encrypt(obj.Password, obj.Email)).ConfigureAwait(false);
            var account = new AccountModel(obj.Name, obj.Password, obj.Email);
            var result = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);

            return result.Success switch {
                true => new CommandResult("The account has been created successfully.", true),
                false => new CommandResult($"There has been an error: {result.ErrorMessage}", false)
            };
        }

        public override string GetDescription() {
            return "This command saves an account that can be later retrieved.";
        }

        protected override AutoValidatorBuilder<AddCommandArgs> AddRules(AutoValidatorBuilder<AddCommandArgs> builder) => builder
            .AddRule<VerifyAccountExistenceRule>(dataCreator);
        
    }
}
