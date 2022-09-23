using PswManager.Core.Cryptography;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner; 
public class AccountCreator : IAccountCreator {

    public AccountCreator(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
        this.dataCreator = dataCreator;
        this.cryptoAccount = cryptoAccount;
    }

    readonly IDataCreator dataCreator;
    readonly ICryptoAccount cryptoAccount;

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

        (model.Password, model.Email) = cryptoAccount.Encrypt(model.Password, model.Email);
        var account = new AccountModel(model.Name, model.Password, model.Email);
        return dataCreator.CreateAccount(account);
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

        (model.Password, model.Email) = await Task.Run(() => cryptoAccount.Encrypt(model.Password, model.Email)).ConfigureAwait(false);
        var account = new AccountModel(model.Name, model.Password, model.Email);
        return await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);
    }

}

