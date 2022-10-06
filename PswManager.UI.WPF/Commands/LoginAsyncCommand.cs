using Microsoft.Extensions.Logging;
using PswManager.Core.Services;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.ViewModels;
using System;
using System.Threading.Tasks;
using static PswManager.Core.Services.ITokenService;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Provides the capability of logging in. To be used when initializing the application after the first time.
/// </summary>
public class LoginAsyncCommand : AsyncCommandBase {

    private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
    private readonly NavigationService<AccountsListingViewModel> _navigationService;
    private readonly ILogger<LoginAsyncCommand>? _logger;
    private readonly INotificationService _notificationService;
    private readonly CryptoContainerService _cryptoContainerService;
    private readonly Func<string> _getPassword;

    public LoginAsyncCommand(Func<string> getPassword,
                            INotificationService notificationService,
                            CryptoContainerService cryptoContainerService,
                            ICryptoAccountServiceFactory cryptoServiceFactory,
                            NavigationService<AccountsListingViewModel> navigationService,
                            ILoggerFactory? loggerFactory = null) {

        _logger = loggerFactory?.CreateLogger<LoginAsyncCommand>();
        _notificationService = notificationService;
        _cryptoContainerService = cryptoContainerService;
        _cryptoAccountServiceFactory = cryptoServiceFactory;
        _getPassword = getPassword;
        _navigationService = navigationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        _notificationService.Send("Validating the password... This might take a few seconds.");
        var result = await _cryptoAccountServiceFactory.LogInAccountAsync(_getPassword.Invoke().ToCharArray());

        var optionResult = result.Result();
        if(optionResult != Utils.Options.OptionResult.Some) {

            var failureCode = result.OrDefaultError();

            if(failureCode is TokenResult.Missing) {
                _logger?.LogCritical("Tried logging in but the token is missing.");
                _notificationService.Send("The token has not been found. " +
                    "Please restart the application and sign up using the same password as before." +
                    "{Environment.NewLine}As changing the password might corrupt all existing data, it's suggested to make a backup copy before signing up again.");
                return;
            }

            var message = failureCode switch {
                TokenResult.Failure => "The given password is incorrect.",
                _ => "The validation has failed for an unknown reason.",
            };

            _notificationService.Send(message);
            return;

        }

        var someResult = result.OrDefault();
        if(someResult == null) {
            _notificationService.Send("There has been an unknown error.");
            return;
        }

        _cryptoContainerService.CryptoAccountService = someResult;
        _navigationService.Navigate(true);

    }

}