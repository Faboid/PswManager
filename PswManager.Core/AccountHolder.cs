using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Core;

internal class AccountHolder : IAccountHolder {

    private EncryptedAccount _encryptedAccount;
    private readonly IAccountValidator _accountValidator;
    private readonly IDataConnection _connection;
    private readonly ILogger<AccountHolder>? _logger;

    public string Name => _encryptedAccount.Name;
    public string Password => _encryptedAccount.Password;
    public string Email => _encryptedAccount.Email;

    public AccountHolder(EncryptedAccount encryptedAccount, IAccountValidator accountValidator, IDataConnection connection, ILoggerFactory? loggerFactory = null) {
        _encryptedAccount = encryptedAccount;
        _accountValidator = accountValidator;
        _connection = connection;
        _logger = loggerFactory?.CreateLogger<AccountHolder>();
    }

    public DecryptedAccount GetDecryptedModel() => _encryptedAccount.GetDecryptedAccount();
    public Task<DecryptedAccount> GetDecryptedModelAsync() => _encryptedAccount.GetDecryptedAccountAsync();

    public async Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues) {

        var newValuesAreValid = _accountValidator.IsAccountValid(newValues);
        if(newValuesAreValid != AccountValid.Valid) {
            return ConvertValidationFailureToEditAccountResult(newValuesAreValid);
        }

        var encrypted = await newValues.GetEncryptedAccountAsync();
        _logger?.LogInformation("Beginning editing {Name}", Name);
        var result = await _connection.UpdateAccountAsync(Name, encrypted);
        if(result != EditorResponseCode.Success) {
            return ReturnConnectionFailure(result);
        }

        LogEditingCompletion(newValues);

        _encryptedAccount = encrypted;
        return EditAccountResult.Success;
    }

    private static EditAccountResult ConvertValidationFailureToEditAccountResult(AccountValid newValuesAreValid) {
        return newValuesAreValid switch {
            AccountValid.NameEmptyOrNull => EditAccountResult.NameCannotBeEmpty,
            AccountValid.PasswordEmptyOrNull => EditAccountResult.PasswordCannotBeEmpty,
            AccountValid.EmailEmptyOrNull => EditAccountResult.EmailCannotBeEmpty,
            _ => EditAccountResult.Unknown,
        };
    }

    private EditAccountResult ReturnConnectionFailure(EditorResponseCode result) {
        var output = result switch {
            EditorResponseCode.NewNameUsedElsewhere => EditAccountResult.NewNameIsOccupied,
            EditorResponseCode.NewNameExistsAlready => EditAccountResult.NewNameIsOccupied,
            _ => EditAccountResult.Unknown,
        };

        var logLevel = LogLevel.Information;
        if(output == EditAccountResult.Unknown) {
            logLevel = LogLevel.Error;
        }

        _logger?.Log(logLevel, "An EditAccount has failed with the result of: {ErrorCode}", result);
        return output;
    }

    private void LogEditingCompletion(IExtendedAccountModel newValues) {
        if(Name != newValues.Name) {
            _logger?.LogInformation("{Name} has been edited successfully to {NewName}", Name, newValues.Name);
        } else {
            _logger?.LogInformation("{Name} edited successfully.", Name);
        }
    }

}
