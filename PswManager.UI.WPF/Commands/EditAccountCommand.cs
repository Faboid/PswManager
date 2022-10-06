using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// When executed, uses the properties from the given <see cref="EditAccountViewModel"/> to attempt updating the account.
/// </summary>
public class EditAccountCommand : AsyncCommandBase {

    private readonly ILogger<EditAccountCommand>? _logger;

    private readonly IAccountModelFactory _accountModelFactory;
    private readonly AccountsStore _accountsStore;
    private readonly EditAccountViewModel _editAccountViewModel;
    private readonly INotificationService _notificationService;
    private readonly NavigationService<AccountsListingViewModel> _ToAccountsListingViewModel;

    public EditAccountCommand(EditAccountViewModel editAccountViewModel, AccountsStore accountsStore, IAccountModelFactory accountModelFactory, INotificationService notificationService, NavigationService<AccountsListingViewModel> toAccountsListingViewModel, ILoggerFactory? loggerFactory = null) {
        _logger = loggerFactory?.CreateLogger<EditAccountCommand>();
        _accountModelFactory = accountModelFactory;
        _accountsStore = accountsStore;
        _editAccountViewModel = editAccountViewModel;
        _notificationService = notificationService;
        _ToAccountsListingViewModel = toAccountsListingViewModel;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        _logger?.LogInformation("Starting the editing operation of {Name}", _editAccountViewModel.CurrentName);
        var name = string.IsNullOrWhiteSpace(_editAccountViewModel.AccountName) ? _editAccountViewModel.CurrentName : _editAccountViewModel.AccountName;
        var password = string.IsNullOrWhiteSpace(_editAccountViewModel.Password) ? _editAccountViewModel.CurrentPassword : _editAccountViewModel.Password;
        var email = string.IsNullOrWhiteSpace(_editAccountViewModel.Email) ? _editAccountViewModel.CurrentEmail : _editAccountViewModel.Email;
        var model = _accountModelFactory.CreateDecryptedAccount(name, password, email);
        var result = await _accountsStore.UpdateAccountAsync(_editAccountViewModel.CurrentName, model);

        var message = result switch {
            UpdateAccountResponse.Success => "The account has been edited successfully.",
            UpdateAccountResponse.NewNameIsEmpty => "The new name cannot be empty.",
            UpdateAccountResponse.AccountNotFound => $"The account {_editAccountViewModel.CurrentName} was not found.",
            UpdateAccountResponse.PasswordEmpty => "The new password cannot be empty.",
            UpdateAccountResponse.EmailEmpty => "The new email cannot be empty.",
            UpdateAccountResponse.NewNameIsOccupied => "The new name is already used by another account.",
            UpdateAccountResponse.NameIsEmpty => "The account name was empty.",
            _ => "The update operation has failed for an unknown reason.",
        };

        _notificationService.Send(message);

        if(result is UpdateAccountResponse.Success) {
            _logger?.LogInformation("{Name}, now {NewName}, has been edited successfully.", _editAccountViewModel.CurrentName, _editAccountViewModel.AccountName);
            _ToAccountsListingViewModel.Navigate(true);
        } else {
            _logger?.LogWarning("{Name}'s editing has failed with the result code of {ErrorCode}", _editAccountViewModel.CurrentName, result);
        }

    }
}