using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Collections;
using PswManager.Core.AccountModels;

namespace PswManager.UI.WPF.ViewModels;

/// <summary>
/// A viewmodel to create a new account.
/// </summary>
public class CreateAccountViewModel : ViewModelBase, INotifyDataErrorInfo {

    private readonly AccountsStore _accountsStore;
    private readonly ErrorsViewModel _errorsViewModel;

    private string _accountName = "";
    public string AccountName {
        get => _accountName;
        set {
            SetAndRaise(nameof(AccountName), ref _accountName, value);

            _errorsViewModel.ClearErrors(nameof(AccountName));
            if(_accountsStore.AccountExists(_accountName)) {
                _errorsViewModel.AddError(nameof(AccountName), "The given name is already in use.");
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

    public CreateAccountViewModel(AccountsStore accountStore, IAccountModelFactory accountModelFactory, INotificationService notificationService, NavigationService<AccountsListingViewModel> navigatorToAccountsListingViewModel) {
        SubmitCommand = new CreateAccountCommand(this, accountStore, accountModelFactory, notificationService, navigatorToAccountsListingViewModel);
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