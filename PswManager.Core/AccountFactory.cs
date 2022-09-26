﻿using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.AccountModels;
using PswManager.Core.Validators;
using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core;

public class AccountFactory : IAccountFactory {

    private readonly ILogger<AccountFactory> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDataConnection _connection;
    private readonly IAccountValidator _accountValidator;
    private readonly IAccountModelFactory _accountModelFactory;
    private readonly Locker _locker = new();

    public AccountFactory(ILoggerFactory loggerFactory, IDataConnection connection, IAccountValidator accountValidator, IAccountModelFactory accountModelFactory) {
        _loggerFactory = loggerFactory;
        _connection = connection;
        _accountValidator = accountValidator;
        _logger = loggerFactory.CreateLogger<AccountFactory>();
        _accountModelFactory = accountModelFactory;
    }

    public async Task<Option<IAccount, CreateAccountErrorCode>> CreateAccountAsync(IAccountModel model) {
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
        _logger.LogInformation("Beginning creation of a new account: {Name}", encrypted.Name);
        var result = await _connection.CreateAccountAsync(encrypted.GetUnderlyingModel()).ConfigureAwait(false);
        if(result.Result() == Utils.Options.OptionResult.Some) {
            _logger.LogInformation("Creation of new account {Name} has failed with error {ErrorCode}", encrypted.Name, result.Or(0));
            return result.Or(0) switch {
                CreatorErrorCode.InvalidName => CreateAccountErrorCode.NameEmptyOrNull,
                CreatorErrorCode.MissingPassword => CreateAccountErrorCode.PasswordEmptyOrNull,
                CreatorErrorCode.MissingEmail => CreateAccountErrorCode.EmailEmptyOrNull,
                CreatorErrorCode.AccountExistsAlready => CreateAccountErrorCode.NameIsOccupied,
                CreatorErrorCode.UsedElsewhere => CreateAccountErrorCode.NameIsOccupied,
                _ => CreateAccountErrorCode.Unknown,
            };
        }

        _logger.LogInformation("A new account, {Name}, has been created successfully.", encrypted.Name);
        var account = NewAccount(encrypted);
        return account;
    }

    public async Task<Option<IAsyncEnumerable<IAccount>>> LoadAccounts() {
        var option = await _connection.GetAllAccountsAsync();

        if(option.Result() != Utils.Options.OptionResult.Some) {
            _logger.LogError("_connection.GetAllAccountsAsync has returned {Result}", option.Result());
            return Option.None<IAsyncEnumerable<IAccount>>();
            //todo - remove the optional return of the DB GetAll
        }

        return new Option<IAsyncEnumerable<IAccount>>(Load(option.Or(AsyncEnumerable.Empty<NamedAccountOption>())));
    }

    private async IAsyncEnumerable<IAccount> Load(IAsyncEnumerable<NamedAccountOption> enumerable) {
        await foreach(var option in enumerable) {
            var info = option.OrDefault();
            if(info != null) {
                yield return NewAccount(_accountModelFactory.CreateEncryptedAccount(info.Name, info.Password, info.Email));
            }
        }
    }

    private Account NewAccount(EncryptedAccount info) => new(info, _connection, _accountValidator, _loggerFactory);

    public enum CreateAccountErrorCode {
        Unknown,
        NameEmptyOrNull,
        PasswordEmptyOrNull,
        EmailEmptyOrNull,
        NameIsOccupied,
    }

}