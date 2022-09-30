using Microsoft.Extensions.Logging;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;

namespace PswManager.UI.WPF.ViewModels;

public class LoginViewModel : ViewModelBase {


	private string _password = "";
	public string Password {
		get { return _password; }
		set { 
			SetAndRaise(nameof(Password), ref _password, value);
			OnPropertyChanged(nameof(CanLogin));
		}
	}

	public bool CanLogin => !string.IsNullOrWhiteSpace(_password);

	public LoginAsyncCommand LoginCommand { get; init; }

	public LoginViewModel(INotificationService notificationService, CryptoContainerService cryptoContainerService, 
						ICryptoAccountServiceFactory cryptoServiceFactory, NavigationService<AccountsListingViewModel> navigationService, 
						ILoggerFactory? loggerFactory = null) {
		LoginCommand = new LoginAsyncCommand(() => _password, notificationService, cryptoContainerService, cryptoServiceFactory, navigationService, loggerFactory);
	}

}