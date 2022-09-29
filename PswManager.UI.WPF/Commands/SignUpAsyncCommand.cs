using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.Services;
using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.ViewModels;
using PswManager.Utils;
using Serilog;
using System;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

public class SignUpAsyncCommand : AsyncCommandBase {

    private readonly NavigationService<AccountsListingViewModel> _navigationService;
    private readonly ILogger<SignUpAsyncCommand>? _logger;
    private readonly INotificationService _notificationService;
    private readonly CryptoContainerService _cryptoContainerService;
    private readonly ITokenService _tokenService;
    private readonly Func<string> _getPassword;

    public SignUpAsyncCommand(Func<string> getPassword, INotificationService notificationService, CryptoContainerService cryptoContainerService, ITokenService tokenService, NavigationService<AccountsListingViewModel> navigationService, ILoggerFactory? loggerFactory = null) {
        _logger = loggerFactory?.CreateLogger<SignUpAsyncCommand>();
        _notificationService = notificationService;
        _cryptoContainerService = cryptoContainerService;
        _tokenService = tokenService;
        _getPassword = getPassword;
        _navigationService = navigationService;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var password = _getPassword.Invoke();

        KeyGeneratorService generatorService = new(password.ToCharArray());
        _notificationService.Send("Setting up everything. This might take a few seconds.");
        var masterKey = await generatorService.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = new CryptoService(masterKey);
        _logger?.LogInformation("Setting up new token.");
        _tokenService.SetToken(cryptoService);
        _logger?.LogInformation("A new token has been set up.");
        _cryptoContainerService.CryptoAccountService = CreateCryptoAccountAsync(generatorService);
        _navigationService.Navigate(true);

    }

    private static ICryptoAccountService CreateCryptoAccountAsync(KeyGeneratorService generator) {
        var lockerReference = new RefCount<Locker>(new());
        return new CryptoAccountService(GenerateKeyAndDisposeGenerator(generator, lockerReference), GenerateKeyAndDisposeGenerator(generator, lockerReference));
    }

    private static async Task<Key> GenerateKeyAndDisposeGenerator(KeyGeneratorService generator, RefCount<Locker> lockerRef) {

        try {
            using var reference = lockerRef.GetRef();
            using var lockhere = await reference.Value.GetLockAsync().ConfigureAwait(false);
            return await generator.GenerateKeyAsync().ConfigureAwait(false);
        } finally {
            if(!lockerRef.IsInUse) {
                Log.Logger.Information("Disposing of KeyGeneratorService...");
                await generator.DisposeAsync().ConfigureAwait(false);
                Log.Logger.Information("Disposed KeyGeneratorService.");
            }
        }
    }

}