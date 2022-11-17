using Microsoft.Extensions.Logging;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using System;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

public class ChangePasswordCommand : AsyncCommandBase {

    private readonly AccountsStore _accountsStore;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly PasswordEditor _passwordEditor;
    private readonly CryptoContainerService _cryptoContainerService;
    private readonly INotificationService _notificationService;
    private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
    private readonly ILogger<ChangePasswordCommand>? _logger;

    public ChangePasswordCommand(SettingsViewModel settingsViewModel, 
                                AccountsStore accountsStore, 
                                PasswordEditor passwordEditor, 
                                CryptoContainerService cryptoContainerService, 
                                INotificationService notificationService, 
                                ICryptoAccountServiceFactory cryptoAccountServiceFactory, 
                                BusyService busyService,
                                ILoggerFactory? loggerFactory = null) : base(busyService) {
        _accountsStore = accountsStore;
        _settingsViewModel = settingsViewModel;
        _passwordEditor = passwordEditor;
        _cryptoContainerService = cryptoContainerService;
        _notificationService = notificationService;
        _cryptoAccountServiceFactory = cryptoAccountServiceFactory;
        _logger = loggerFactory?.CreateLogger<ChangePasswordCommand>();
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var newPassword = _settingsViewModel.NewPassword;

        _logger?.LogInformation("Requested change of password.");
        _notificationService.Send("The password-changing operation is starting. Please don't close the application.");
        var result = await _passwordEditor.ChangePasswordTo(newPassword);

        //if success
        if(result is PasswordChangeResult.Success) {

            _logger?.LogInformation("The password has been changed successfully.");
            var loginResult = await _cryptoAccountServiceFactory.LogInAccountAsync(newPassword.ToCharArray());

            var cryptoAccountService = loginResult.OrDefault();
            if(cryptoAccountService is null) {

                _logger?.LogInformation("The password has been changed successfully, but logging in as failed with the error of {ErrorCode}.", loginResult.OrError(0));
                throw new InvalidOperationException($"The password has been changed successfully, but the log-in has failed with the error of {loginResult.OrError(0)}.");

            }

            _cryptoContainerService.CryptoAccountService = cryptoAccountService;
            _notificationService.Send("The password has been changed successfully.");
            _accountsStore.Reset();
            return;

        }

        //if error
        _logger?.LogInformation("The password-changing operation has failed with the result of {ResultCode}.", result);
        _notificationService.Send("There has been an error when changing the password.");

    }
}