using Microsoft.Extensions.Logging;
using PswManager.Core.Services;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.ConstantValues;
using PswManager.UI.WPF.Services;

namespace PswManager.UI.WPF.ViewModels;

/// <summary>
/// Viewmodel that handles login in the application and setting up the cryptography master key.
/// </summary>
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

#if DEBUG
		Password = Debugging.MasterKey;
#endif

	}

}