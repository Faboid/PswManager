using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.AccountModels;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Core;

internal class Account : IAccount {

    private readonly ILogger<Account>? _logger;
    private readonly IDataConnection _connection;
    private readonly IAccountValidator _accountValidator;
    private readonly Locker _locker = new();

    private bool _exists = true;
    private EncryptedAccount _encryptedAccount;
    public string Name => _encryptedAccount.Name;
    public string EncryptedPassword => _encryptedAccount.Password;
    public string EncryptedEmail => _encryptedAccount.Email;

    public Account(EncryptedAccount model, IDataConnection connection, IAccountValidator accountValidator, ILoggerFactory? loggerFactory = null) {
        _encryptedAccount = model;
        _logger = loggerFactory?.CreateLogger<Account>();
        _connection = connection;
        _accountValidator = accountValidator;
    }

    public DecryptedAccount GetDecryptedModel() => _encryptedAccount.GetDecryptedAccount();
    public Task<DecryptedAccount> GetDecryptedModelAsync() => _encryptedAccount.GetDecryptedAccountAsync();

    public async Task<EditAccountResult> EditAccountAsync(IAccountModel newValues) {
        using var locker = await _locker.GetLockAsync();

        if(!_exists) { return EditAccountResult.DoesNotExist; }

        var newValuesAreValid = _accountValidator.IsAccountValid(newValues);
        if(newValuesAreValid != AccountValid.Valid) {
            return newValuesAreValid switch {
                AccountValid.NameEmptyOrNull => EditAccountResult.NameCannotBeEmpty,
                AccountValid.PasswordEmptyOrNull => EditAccountResult.PasswordCannotBeEmpty,
                AccountValid.EmailEmptyOrNull => EditAccountResult.EmailCannotBeEmpty,
                _ => EditAccountResult.Unknown,
            };
        }

        var encrypted = await newValues.GetEncryptedAccountAsync();
        _logger?.LogInformation("Beginning editing {Name}", Name);
        var result = await _connection.UpdateAccountAsync(Name, encrypted.GetUnderlyingModel());
        if(result != EditorResponseCode.Success) {
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

        if(Name != newValues.Name) {
            _logger?.LogInformation("{Name} has been edited successfully to {NewName}", Name, newValues.Name);
        } else {
            _logger?.LogInformation("{Name} edited successfully.", Name);
        }

        _encryptedAccount = encrypted;
        return EditAccountResult.Success;
    }

    public async Task DeleteAccountAsync() {

        using var locker = await _locker.GetLockAsync();
        _logger?.LogInformation("Starting to delete {Name}.", Name);
        _ = await _connection.DeleteAccountAsync(Name);
        _logger?.LogInformation("{Name} has been deleted.", Name);
        _exists = false;

    }

}
