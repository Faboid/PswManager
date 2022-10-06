using Microsoft.Extensions.Logging;
using PswManager.Core;
using PswManager.Extensions;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

/// <summary>
/// Represents a list of accounts. Acts as the "home" viewmodel of this application.
/// </summary>
public class AccountsListingViewModel : ViewModelBase {

    private readonly AccountsStore _accountsStore;
    private readonly ICollectionView _collectionView;
    private readonly Func<IAccount, AccountViewModel> _createAccountViewModel;
    private readonly ObservableCollection<AccountViewModel> _accounts = new();
    public IEnumerable<AccountViewModel> Accounts => _accounts;

    private string _search = "";
    public string Search {
        get { return _search; }
        set { 
            SetAndRaise(nameof(Search), ref _search, value);
            _collectionView.Refresh();
        }
    }

    private AccountViewModel? _closeUpViewModel = null;
    public AccountViewModel? CloseUpViewModel {
        get => _closeUpViewModel;
        set {
            SetAndRaise(nameof(CloseUpViewModel), ref _closeUpViewModel, value);
            OnPropertyChanged(nameof(ShowDetails));
        }
    }

    public bool ShowDetails => CloseUpViewModel?.VisibleDetails ?? false;

    public ICommand SettingsCommand { get; }
    public ICommand CreateAccountCommand { get; }
    public ICommand LoadAccountsCommand { get; }

    public AccountsListingViewModel(AccountsStore accountsStore, INotificationService notificationService, 
                                    Func<IAccount, AccountViewModel> createAccountViewModel, 
                                    NavigationService<SettingsViewModel> navigationServiceToSettingsViewModel, NavigationService<CreateAccountViewModel> navigationServiceToCreateAccountViewModel, 
                                    ILoggerFactory? loggerFactory = null) {
        _createAccountViewModel = createAccountViewModel;
        _accountsStore = accountsStore;
        SettingsCommand = new NavigateCommand<SettingsViewModel>(true, navigationServiceToSettingsViewModel);
        CreateAccountCommand = new NavigateCommand<CreateAccountViewModel>(true, navigationServiceToCreateAccountViewModel);
        LoadAccountsCommand = new LoadAccountsCommand(accountsStore, notificationService, LoadAccounts, loggerFactory);
        _accountsStore.AccountsChanged += LoadAccounts;

        _collectionView = CollectionViewSource.GetDefaultView(_accounts);
        _collectionView.Filter += AccountsFilter;
    }

    private bool AccountsFilter(object obj) => obj is AccountViewModel account && account.Name.Contains(_search);
    private void LoadAccounts() => LoadAccounts(_accountsStore.Accounts);
    private void LoadAccounts(IEnumerable<IAccount> accounts) {
        CloseAccountDetails();
        UnsubscribeFromAccounts();
        _accounts.Clear();
        foreach(var account in accounts) {
            var vm = _createAccountViewModel.Invoke(account);
            vm.ShowDetails += ShowAccountDetails;
            vm.CloseDetails += CloseAccountDetails;
            _accounts.Add(vm);
        }
    }

    private void ShowAccountDetails(AccountViewModel obj) => CloseUpViewModel = obj;
    private void CloseAccountDetails() => CloseUpViewModel = null;

    private void UnsubscribeFromAccounts() {
        _accounts.ForEach(x => {
            x.ShowDetails -= ShowAccountDetails;
            x.CloseDetails -= CloseAccountDetails;
        });
    }

    protected override void Dispose(bool disposed) {
        _accountsStore.AccountsChanged -= LoadAccounts;
        UnsubscribeFromAccounts();
        _collectionView.Filter -= AccountsFilter;
        base.Dispose(disposed);
    }
}