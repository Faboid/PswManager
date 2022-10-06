using Microsoft.Extensions.Logging;
using PswManager.Core.AccountModels;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

/// <summary>
/// A viewmodel to edit an account.
/// </summary>
public class EditAccountViewModel : ViewModelBase, INotifyDataErrorInfo {

    private readonly DecryptedAccount _account;
    private readonly AccountsStore _accountsStore;
    private readonly ErrorsViewModel _errorsViewModel;

    public string CurrentName => _account.Name;
    public string CurrentPassword => _account.Password;
    public string CurrentEmail => _account.Email;

    private string _accountName = "";
    public string AccountName {
        get => _accountName;
        set {
            SetAndRaise(nameof(AccountName), ref _accountName, value);

            _errorsViewModel.ClearErrors(nameof(AccountName));
            if(_accountsStore.AccountExists(_accountName)) {
                _errorsViewModel.AddError(nameof(AccountName), "The new name is already in use.");
            }
            if(string.IsNullOrWhiteSpace(_accountName)) {
                _errorsViewModel.AddError(nameof(AccountName), "The name cannot be empty.");
            }
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    private string _password = "";
    public string Password {
        get { return _password; }
        set {
            SetAndRaise(nameof(Password), ref _password, value);
            _errorsViewModel.ClearErrors(nameof(Password));
            if(string.IsNullOrEmpty(_password)) {
                _errorsViewModel.AddError(nameof(Password), "The password cannot be empty.");
            }
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    private string _email = "";

    public string Email {
        get { return _email; }
        set {
            SetAndRaise(nameof(Email), ref _email, value);
            _errorsViewModel.ClearErrors(nameof(Email));
            if(string.IsNullOrWhiteSpace(Email)) {
                _errorsViewModel.AddError(nameof(Email), "The email cannot be empty.");
            }
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }

    public EditAccountViewModel(DecryptedAccount account, AccountsStore accountStore, IAccountModelFactory accountModelFactory, INotificationService notificationService, NavigationService<AccountsListingViewModel> navigatorToAccountsListingViewModel, ILoggerFactory? loggerFactory = null) {
        _account = account;
        SubmitCommand = new EditAccountCommand(this, accountStore, accountModelFactory, notificationService, navigatorToAccountsListingViewModel, loggerFactory);
        CancelCommand = new NavigateCommand<AccountsListingViewModel>(true, navigatorToAccountsListingViewModel);
        _accountsStore = accountStore;
        _errorsViewModel = new ErrorsViewModel();
        _errorsViewModel.ErrorsChanged += OnErrorsChanged;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) {
        ErrorsChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(CanCreate));
    }

    public bool CanCreate => !HasErrors && !string.IsNullOrWhiteSpace(AccountName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email);
    public bool HasErrors => _errorsViewModel.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) {
        return _errorsViewModel.GetErrors(propertyName);
    }

    protected override void Dispose(bool disposed) {
        _errorsViewModel.ErrorsChanged -= OnErrorsChanged;
        base.Dispose(disposed);
    }

}