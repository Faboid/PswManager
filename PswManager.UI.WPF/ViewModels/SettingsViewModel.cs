using Microsoft.Extensions.Logging;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
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
        set { SetAndRaise(nameof(NewPassword), ref _newPassword, value); }
    }

    public SettingsViewModel(NavigationService<AccountsListingViewModel> navigationServiceToListingViewModel, 
                            PasswordEditor passwordEditor, CryptoContainerService cryptoContainerService, 
                            INotificationService notificationService, 
                            ICryptoAccountServiceFactory cryptoAccountServiceFactory, 
                            ILoggerFactory? loggerFactory) {

        HomeButton = new NavigateCommand<AccountsListingViewModel>(true, navigationServiceToListingViewModel);
        ChangePasswordCommand = new ChangePasswordCommand(this, passwordEditor, cryptoContainerService, notificationService, cryptoAccountServiceFactory, loggerFactory);
    }

}