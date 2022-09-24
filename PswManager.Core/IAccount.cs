using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.Inner.Interfaces;
using PswManager.Core.Services;
using PswManager.Core.Validators;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccount {


}

public class AccountFactory {

    public void CreateAccount() {

    }

    public void LoadAccounts() {

    }

}

internal class Account {

    private readonly ILogger<IAccount> _logger;
    private readonly IAccountEditor _accountEditor;
    private readonly IAccountDeleter _accountDeleter;
    private readonly IAccountValidator _accountValidator;
    private readonly ICryptoAccountService _cryptoAccountService;
    private readonly Locker _locker;

    private AccountModel _accountInfo;
    public string Name => _accountInfo.Name;

    public AccountModel GetDecryptedModel() => _cryptoAccountService.Decrypt(_accountInfo);

    public async Task<EditAccountResult> EditAccountAsync(AccountModel newValues) {
        using var locker = await _locker.GetLockAsync();

        var newValuesAreValid = _accountValidator.IsAccountValid(newValues);
        if(newValuesAreValid != AccountValid.Valid) {
            return newValuesAreValid switch {
                AccountValid.NameEmptyOrNull => EditAccountResult.NameCannotBeEmpty,
                AccountValid.PasswordEmptyOrNull => EditAccountResult.PasswordCannotBeEmpty,
                AccountValid.EmailEmptyOrNull => EditAccountResult.EmailCannotBeEmpty,
                _ => EditAccountResult.Unknown,
            };
        }

        _logger.LogInformation("Beginning editing {Name}", Name);
        var result = await _accountEditor.UpdateAccountAsync(Name, newValues);
        if(result.Result() == Utils.Options.OptionResult.Some) {
            var resultCode = result.Or(0);
            var output = resultCode switch {
                EditorErrorCode.NewNameUsedElsewhere => throw new System.NotImplementedException(),
                EditorErrorCode.NewNameExistsAlready => throw new System.NotImplementedException(),
                _ => EditAccountResult.Unknown,
            };

            var logLevel = LogLevel.Information;
            if(output == EditAccountResult.Unknown) {
                logLevel = LogLevel.Error;
            }

            _logger.Log(logLevel, "An EditAccount operation returned: {ErrorCode}", resultCode);
            return output;
        }

        if(Name != newValues.Name) {
            _logger.LogInformation("{Name} has been edited successfully to {NewName}", Name, newValues.Name);
        } else {
            _logger.LogInformation("{Name} edited successfully.", Name);
        }

        _accountInfo = new AccountModel(newValues.Name, newValues.Password, newValues.Email);
        return EditAccountResult.Success;
    }

    public async Task DeleteAccount() {

        using var locker = await _locker.GetLockAsync();

        throw new NotImplementedException();

    }

}

public enum EditAccountResult {
    Unknown,
    Success,
    NameCannotBeEmpty,
    PasswordCannotBeEmpty,
    EmailCannotBeEmpty,
}
