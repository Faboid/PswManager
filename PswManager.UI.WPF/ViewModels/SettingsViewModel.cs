using Microsoft.Extensions.Logging;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

/// <summary>
/// Currently not implemented. Represents a viewmodel to change application settings.
/// </summary>
public class SettingsViewModel : ViewModelBase {

    public ICommand ChangePasswordCommand { get; }
    public ICommand HomeButton { get; }

    private string _newPassword = "";
    public string NewPassword {
        get { return _newPassword; }
        set { 
            SetAndRaise(nameof(NewPassword), ref _newPassword, value);
            OnPropertyChanged(nameof(CanChangePassword));
        }
    }

    private string _repeatNewPassword = "";
    public string RepeatNewPassword {
        get { return _repeatNewPassword; }
        set { 
            SetAndRaise(nameof(RepeatNewPassword), ref _repeatNewPassword, value);
            OnPropertyChanged(nameof(CanChangePassword));
        }
    }

    public SettingsViewModel(NavigationService<AccountsListingViewModel> navigationServiceToListingViewModel, 
                            AccountsStore accountsStore,
                            PasswordEditor passwordEditor, CryptoContainerService cryptoContainerService, 
                            INotificationService notificationService, 
                            ICryptoAccountServiceFactory cryptoAccountServiceFactory, 
                            ILoggerFactory? loggerFactory) {

        var busyService = new BusyService();
        HomeButton = new NavigateCommand<AccountsListingViewModel>(true, navigationServiceToListingViewModel, busyService);
        ChangePasswordCommand = new ChangePasswordCommand(this, accountsStore, passwordEditor, cryptoContainerService, notificationService, cryptoAccountServiceFactory, busyService, loggerFactory);
    }

    public bool CanChangePassword => !string.IsNullOrWhiteSpace(NewPassword) && NewPassword == RepeatNewPassword;

}