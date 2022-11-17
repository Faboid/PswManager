using Microsoft.Extensions.Logging;
using PswManager.Core;
using PswManager.Core.AccountModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PswManager.Core.IAccountFactory;

namespace PswManager.UI.WPF.Stores;

/// <summary>
/// Provides a centralized place to keep, load, edit, delete, and create accounts.
/// </summary>
public class AccountsStore {

    private readonly ILogger<AccountsStore>? _logger;
    private readonly IAccountFactory _accountFactory;
    private readonly Dictionary<string, IAccount> _accounts = new();
    private Lazy<Task> _initializationTask;

    /// <summary>
    /// The accounts loaded in cache. Will be empty if called before <see cref="Load"/> has finished.
    /// </summary>
    public IEnumerable<IAccount> Accounts => _accounts.Values;

    /// <summary>
    /// Raised when one or more accounts are created, deleted, edited, or loaded.
    /// </summary>
    public event Action? AccountsChanged;

    /// <summary>
    /// Initializes <see cref="AccountsStore"/>.
    /// </summary>
    /// <param name="accountFactory"></param>
    /// <param name="loggerFactory"></param>
    public AccountsStore(IAccountFactory accountFactory, ILoggerFactory? loggerFactory = null) {
        _initializationTask = new(Initialize);
        _logger = loggerFactory?.CreateLogger<AccountsStore>();
        _accountFactory = accountFactory;
    }

    private void OnAccountsChanged() => AccountsChanged?.Invoke();

    /// <summary>
    /// Loads the accounts from the database. Will return immediately if they have already been loaded.
    /// </summary>
    /// <returns></returns>
    public Task Load() => _initializationTask.Value;

    /// <summary>
    /// Erases all cached data.
    /// </summary>
    public void Reset() {
        _accounts.Clear();
        OnAccountsChanged();
        _initializationTask = new(Initialize);
    }

    private async Task Initialize() {

        var accounts = _accountFactory.LoadAccounts();
        _accounts.Clear();
        await foreach(var account in accounts) {
            _accounts.Add(account.Name, account);
        }
        OnAccountsChanged();
        _logger?.LogInformation("The accounts have been loaded.");
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Whether the account exists in the cache.</returns>
    public bool AccountExists(string name) => _accounts.ContainsKey(name);

    /// <summary>
    /// Attempts creating an account with the given values and returns the result.
    /// </summary>
    /// <param name="accountModel">The values used to create the new account.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<CreateAccountResponse> CreateAccountAsync(IExtendedAccountModel accountModel) {

        if(AccountExists(accountModel.Name)) { return CreateAccountResponse.NameIsOccupied; }

        _logger?.LogInformation("Beginning to create {Name} account.", accountModel.Name);
        var optionResult = await _accountFactory.CreateAccountAsync(accountModel);

        return optionResult.Match(
            some => {
                _accounts.Add(some.Name, some);
                OnAccountsChanged();
                _logger?.LogInformation("{Name} has been created successfully.", accountModel.Name);
                return CreateAccountResponse.Success;
            },
            error => {

                _logger?.LogInformation("The creation of {Name} has failed with the error of {ErrorCode}", accountModel.Name, error);
                return error switch {
                    CreateAccountErrorCode.NameEmptyOrNull => CreateAccountResponse.NameEmpty,
                    CreateAccountErrorCode.PasswordEmptyOrNull => CreateAccountResponse.PasswordEmpty,
                    CreateAccountErrorCode.EmailEmptyOrNull => CreateAccountResponse.EmailEmpty,
                    CreateAccountErrorCode.NameIsOccupied => CreateAccountResponse.NameIsOccupied,
                    _ => CreateAccountResponse.Unknown,
                };
            },
            () => {
                _logger?.LogWarning("The creation of {Name} has failed with a return of None.", accountModel.Name);
                return CreateAccountResponse.Unknown;
            });
    }

    /// <summary>
    /// Attempts updating the account <paramref name="name"/> with the given <paramref name="model"/> values.
    /// </summary>
    /// <param name="name">The name of the account to be edited.</param>
    /// <param name="model">The model containing the new values.</param>
    /// <returns>A <see cref="UpdateAccountResponse"/> detailing the result of the operation.</returns>
    public async Task<UpdateAccountResponse> UpdateAccountAsync(string name, IExtendedAccountModel model) {

        if(string.IsNullOrWhiteSpace(name)) {
            return UpdateAccountResponse.NameIsEmpty;
        }

        if(!_accounts.TryGetValue(name, out var account)) {
            return UpdateAccountResponse.AccountNotFound;
        }

        _logger?.LogInformation("Beginning to edit the account {Name}", name);
        var result = await account.EditAccountAsync(model);

        if(result is EditAccountResult.Success) {
            OnAccountsChanged();
            _logger?.LogInformation("{Name}, now {NewName}, has been edited successfully.", name, model.Name);
        } else {
            _logger?.LogInformation("The editing of {Name} has failed with the error code {ErrorCode}", name, result);
        }

        return result switch {
            EditAccountResult.Success => UpdateAccountResponse.Success,
            EditAccountResult.NameCannotBeEmpty => UpdateAccountResponse.NewNameIsEmpty,
            EditAccountResult.PasswordCannotBeEmpty => UpdateAccountResponse.PasswordEmpty,
            EditAccountResult.EmailCannotBeEmpty => UpdateAccountResponse.EmailEmpty,
            EditAccountResult.NewNameIsOccupied => UpdateAccountResponse.NewNameIsOccupied,
            EditAccountResult.DoesNotExist => UpdateAccountResponse.AccountNotFound,
            _ => UpdateAccountResponse.Unknown,
        };
    }

    /// <summary>
    /// Attempts deleting the account named <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the account to delete.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    public async Task<DeleteAccountResponse> DeleteAccountAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleteAccountResponse.NameCannotBeEmptyOrNull;
        }

        if(!_accounts.TryGetValue(name, out var account)) {
            return DeleteAccountResponse.AccountNotFound;
        }

        try {
            await account.DeleteAccountAsync();
            _accounts.Remove(name);
            OnAccountsChanged();
            return DeleteAccountResponse.Success;

        } catch(Exception ex) {

            _logger?.LogError(ex, "An exception has been thrown when trying to delete {Name}", name);
            return DeleteAccountResponse.Failure;

        }

    }

}

/// <summary>
/// Represents the result of a <see cref="AccountsStore.CreateAccountAsync(IExtendedAccountModel)"/> operation.
/// </summary>
public enum CreateAccountResponse {
    Unknown,
    Success,
    Failure,
    NameEmpty,
    PasswordEmpty,
    EmailEmpty,
    NameIsOccupied,
}

/// <summary>
/// Represents the result of a <see cref="AccountsStore.UpdateAccountAsync(string, IExtendedAccountModel)"/> operation.
/// </summary>
public enum UpdateAccountResponse {
    Unknown,
    Success,
    NewNameIsEmpty,
    AccountNotFound,
    PasswordEmpty,
    EmailEmpty,
    NewNameIsOccupied,
    NameIsEmpty,
}

/// <summary>
/// Represents the result of a <see cref="AccountsStore.DeleteAccountAsync(string)"/> operation.
/// </summary>
public enum DeleteAccountResponse {
    Unknown,
    Success,
    NameCannotBeEmptyOrNull,
    AccountNotFound,
    Failure
}