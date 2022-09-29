using Microsoft.Extensions.Logging;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

public class FirstTimeLoginViewModel : ViewModelBase {

	private string _Password = "";
	public string Password {
		get { return _Password; }
		set { SetAndRaise(nameof(Password), ref _Password, value); }
	}

	private string _repeatPassword = "";
	public string RepeatPassword {
		get { return _repeatPassword; }
		set { SetAndRaise(nameof(RepeatPassword), ref _repeatPassword, value); }
	}

	public ICommand SendCommand { get; }

    public FirstTimeLoginViewModel(CryptoContainerService cryptoContainerService, INotificationService notificationService, ITokenService tokenService, NavigationService<AccountsListingViewModel> navigationToListingViewModel, ILoggerFactory? loggerFactory = null) {
		SendCommand = new SignUpAsyncCommand(() => Password, notificationService, cryptoContainerService, tokenService, navigationToListingViewModel, loggerFactory);
    }

}