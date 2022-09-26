using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner;
public class AccountCreator : IAccountCreator {

    public AccountCreator(IDataCreator dataCreator, ICryptoAccountService cryptoAccount) {
        this.dataCreator = dataCreator;
        this.cryptoAccount = cryptoAccount;
    }

    readonly IDataCreator dataCreator;
    readonly ICryptoAccountService cryptoAccount;

    public Option<CreatorErrorCode> CreateAccount(AccountModel model) {

        var validationResult = model.IsAnyValueNullOrEmpty();
        if(validationResult != ValidationResult.Success) {
            return validationResult switch {
                ValidationResult.MissingName => CreatorErrorCode.InvalidName,
                ValidationResult.MissingPassword => CreatorErrorCode.MissingPassword,
                ValidationResult.MissingEmail => CreatorErrorCode.MissingEmail,
                _ => CreatorErrorCode.Undefined,
            };
        }

        var account = new AccountModel(model.Name, model.Password, model.Email);
        (account.Password, account.Email) = cryptoAccount.Encrypt(account.Password, account.Email);
        return dataCreator.CreateAccountAsync(account).GetAwaiter().GetResult();
    }

    public async Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model) {

        var validationResult = model.IsAnyValueNullOrEmpty();
        if(validationResult != ValidationResult.Success) {
            return validationResult switch {
                ValidationResult.MissingName => CreatorErrorCode.InvalidName,
                ValidationResult.MissingPassword => CreatorErrorCode.MissingPassword,
                ValidationResult.MissingEmail => CreatorErrorCode.MissingEmail,
                _ => CreatorErrorCode.Undefined,
            };
        }

        var account = new AccountModel(model.Name, model.Password, model.Email);
        (account.Password, account.Email) = await Task.Run(() => cryptoAccount.Encrypt(account.Password, account.Email)).ConfigureAwait(false);
        return await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);
    }

}

