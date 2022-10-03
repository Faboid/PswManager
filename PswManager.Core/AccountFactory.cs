using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.AccountModels;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PswManager.Core.IAccountFactory;

namespace PswManager.Core;

public class AccountFactory : IAccountFactory {

    private readonly ILogger<AccountFactory>? _logger;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly IDataConnection _connection;
    private readonly IAccountValidator _accountValidator;
    private readonly IAccountModelFactory _accountModelFactory;
    private readonly Locker _locker = new();

    public AccountFactory(IDataConnection connection, IAccountValidator accountValidator, IAccountModelFactory accountModelFactory, ILoggerFactory? loggerFactory = null) {
        _loggerFactory = loggerFactory;
        _connection = connection;
        _accountValidator = accountValidator;
        _logger = loggerFactory?.CreateLogger<AccountFactory>();
        _accountModelFactory = accountModelFactory;
    }

    public async Task<Option<IAccount, CreateAccountErrorCode>> CreateAccountAsync(IExtendedAccountModel model) {
        var valid = _accountValidator.IsAccountValid(model);
        if(valid != AccountValid.Valid) {
            return valid switch {
                AccountValid.NameEmptyOrNull => CreateAccountErrorCode.NameEmptyOrNull,
                AccountValid.PasswordEmptyOrNull => CreateAccountErrorCode.PasswordEmptyOrNull,
                AccountValid.EmailEmptyOrNull => CreateAccountErrorCode.EmailEmptyOrNull,
                _ => CreateAccountErrorCode.Unknown,
            };
        }

        var encryptedTask = model.GetEncryptedAccountAsync().ConfigureAwait(false);
        using var locker = await _locker.GetLockAsync();
        var encrypted = await encryptedTask;
        _logger?.LogInformation("Beginning creation of a new account: {Name}", encrypted.Name);
        var result = await _connection.CreateAccountAsync(encrypted).ConfigureAwait(false);
        if(result != CreatorResponseCode.Success) {
            _logger?.LogInformation("Creation of new account {Name} has failed with error {ErrorCode}", encrypted.Name, result);
            return result switch {
                CreatorResponseCode.InvalidName => CreateAccountErrorCode.NameEmptyOrNull,
                CreatorResponseCode.MissingPassword => CreateAccountErrorCode.PasswordEmptyOrNull,
                CreatorResponseCode.MissingEmail => CreateAccountErrorCode.EmailEmptyOrNull,
                CreatorResponseCode.AccountExistsAlready => CreateAccountErrorCode.NameIsOccupied,
                CreatorResponseCode.UsedElsewhere => CreateAccountErrorCode.NameIsOccupied,
                _ => CreateAccountErrorCode.Unknown,
            };
        }

        _logger?.LogInformation("A new account, {Name}, has been created successfully.", encrypted.Name);
        var account = NewAccount(encrypted);
        return account;
    }

    public async IAsyncEnumerable<IAccount> LoadAccounts() {
        var enumerable = _connection.EnumerateAccountsAsync();
        await foreach(var option in enumerable) {
            var info = option.OrDefault(); //todo - turn corrupted accounts into valid warnings
            if(info != null) {
                yield return NewAccount(_accountModelFactory.CreateEncryptedAccount(info.Name, info.Password, info.Email));
            }
        }
    }

    private Account NewAccount(EncryptedAccount info) => new(info, _connection, _accountValidator, _loggerFactory);

}
