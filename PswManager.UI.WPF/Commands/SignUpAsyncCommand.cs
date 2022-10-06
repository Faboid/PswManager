using PswManager.Core.Services;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.ViewModels;
using System;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Provides a way to set up the application's encryption capabilities.
/// </summary>
public class SignUpAsyncCommand : AsyncCommandBase {

    private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;    
    private readonly NavigationService<AccountsListingViewModel> _navigationService;
    private readonly INotificationService _notificationService;
    private readonly CryptoContainerService _cryptoContainerService;
    private readonly Func<string> _getPassword;

    public SignUpAsyncCommand(Func<string> getPassword, 
                            INotificationService notificationService, CryptoContainerService cryptoContainerService, 
                            ICryptoAccountServiceFactory cryptoAccountServiceFactory, 
                            NavigationService<AccountsListingViewModel> navigationService) {

        _notificationService = notificationService;
        _cryptoAccountServiceFactory = cryptoAccountServiceFactory;
        _cryptoContainerService = cryptoContainerService;
        _getPassword = getPassword;
        _navigationService = navigationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        _notificationService.Send("Setting up everything. This might take a few seconds.");
        var result = await _cryptoAccountServiceFactory.SignUpAccountAsync(_getPassword.Invoke().ToCharArray());
        _cryptoContainerService.CryptoAccountService = result;
        _navigationService.Navigate(true);

    }
}