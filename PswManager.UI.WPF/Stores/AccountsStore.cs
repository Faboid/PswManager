using Microsoft.Extensions.Logging;
using PswManager.Core;
using PswManager.Core.AccountModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PswManager.Core.IAccountFactory;

namespace PswManager.UI.WPF.Stores;

public class AccountsStore {

    private readonly ILogger<AccountsStore>? _logger;
    private readonly IAccountFactory _accountFactory;
    private readonly Dictionary<string, IAccount> _accounts = new();
    public IEnumerable<IAccount> Accounts => _accounts.Values;
    private readonly Lazy<Task> _initializationTask;

    public event Action? AccountsChanged;

    public AccountsStore(IAccountFactory accountFactory, ILoggerFactory? loggerFactory = null) {
        _initializationTask = new(Initialize);
        _logger = loggerFactory?.CreateLogger<AccountsStore>();
        _accountFactory = accountFactory;
    }

    private void OnAccountsChanged() => AccountsChanged?.Invoke();

    public Task Load() => _initializationTask.Value;

    private async Task Initialize() {

        var accounts = _accountFactory.LoadAccounts();
        _accounts.Clear();
        await foreach(var account in accounts) {
            _accounts.Add(account.Name, account);
        }
        OnAccountsChanged();
        _logger?.LogInformation("The accounts have been loaded.");
    }

    public bool AccountExists(string name) => _accounts.ContainsKey(name);

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

public enum CreateAccountResponse {
    Unknown,
    Success,
    Failure,
    NameEmpty,
    PasswordEmpty,
    EmailEmpty,
    NameIsOccupied,
}

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

public enum DeleteAccountResponse {
    Unknown,
    Success,
    NameCannotBeEmptyOrNull,
    AccountNotFound,
    Failure
}