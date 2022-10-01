using PswManager.Core;
using PswManager.Core.AccountModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Stores;

public class AccountsStore {
    
    private readonly IAccountFactory _accountFactory;
    private readonly Dictionary<string, IAccount> _accounts = new();
    public IEnumerable<IAccount> Accounts => _accounts.Values;
    private readonly Lazy<Task> _initializationTask;

    public event Action? AccountsChanged;

    public AccountsStore(IAccountFactory accountFactory) {
        _initializationTask = new(Initialize);
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
    }

    public bool AccountExists(string name) => _accounts.ContainsKey(name);

    public async Task<CreateAccountResponse> CreateAccountAsync(IExtendedAccountModel accountModel) {

        if(AccountExists(accountModel.Name)) { return CreateAccountResponse.NameIsOccupied; }

        var optionResult = await _accountFactory.CreateAccountAsync(accountModel);

        return optionResult.Match(
            some => {
                _accounts.Add(some.Name, some);
                OnAccountsChanged();
                return CreateAccountResponse.Success;
            },
            error => {
                return error switch {
                    AccountFactory.CreateAccountErrorCode.NameEmptyOrNull => CreateAccountResponse.NameEmpty,
                    AccountFactory.CreateAccountErrorCode.PasswordEmptyOrNull => CreateAccountResponse.PasswordEmpty,
                    AccountFactory.CreateAccountErrorCode.EmailEmptyOrNull => CreateAccountResponse.EmailEmpty,
                    AccountFactory.CreateAccountErrorCode.NameIsOccupied => CreateAccountResponse.NameIsOccupied,
                    _ => CreateAccountResponse.Unknown,
                };
            },
            () => CreateAccountResponse.Unknown
        );
    }

    public async Task<UpdateAccountResponse> UpdateAccountAsync(string name, IExtendedAccountModel model) {

        if(string.IsNullOrWhiteSpace(name)) {
            return UpdateAccountResponse.NameIsEmpty;
        }

        if(!_accounts.TryGetValue(name, out var account)) {
            return UpdateAccountResponse.AccountNotFound;
        }

        var result = await account.EditAccountAsync(model);

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