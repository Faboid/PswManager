using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using PswManager.Utils;
using System;
using System.Threading.Tasks;
using static PswManager.Core.Services.ITokenService;

namespace PswManager.Core.Services;

public class CryptoAccountServiceFactory : ICryptoAccountServiceFactory {

    private readonly ILogger<CryptoAccountServiceFactory>? _logger;
    private readonly ICryptoServiceFactory _cryptoServiceFactory;
    private readonly ITokenService _tokenService;
    private readonly Func<char[], KeyGeneratorService> _createGeneratorService;

    public CryptoAccountServiceFactory(ITokenService tokenService, ICryptoServiceFactory cryptoServiceFactory, ILoggerFactory? loggerFactory = null) {
        _logger = loggerFactory?.CreateLogger<CryptoAccountServiceFactory>();
        _tokenService = tokenService;
        _createGeneratorService = x => new(x);
        _cryptoServiceFactory = cryptoServiceFactory;
    }

    /// <summary>
    /// This constructor is meant to be used only for tests. Do not use outside of them.
    /// </summary>
    /// <param name="customGeneratorServiceFunc"></param>
    internal CryptoAccountServiceFactory(ITokenService tokenService, ICryptoServiceFactory cryptoServiceFactory, Func<char[], KeyGeneratorService> customGeneratorServiceFunc) {
        _createGeneratorService = customGeneratorServiceFunc;
        _tokenService = tokenService;
        _cryptoServiceFactory = cryptoServiceFactory;
    }

    public async Task<ICryptoAccountService> SignUpAccountAsync(char[] password) {

        KeyGeneratorService generatorService = _createGeneratorService.Invoke(password);
        var masterKey = await generatorService.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = _cryptoServiceFactory.GetCryptoService(masterKey);
        _logger?.LogInformation("Setting up new token.");
        _tokenService.SetToken(cryptoService);
        _logger?.LogInformation("A new token has been set up.");
        return CreateCryptoAccountAsync(generatorService);

    }


    public async Task<Option<ICryptoAccountService, TokenResult>> LogInAccountAsync(char[] password) {


        KeyGeneratorService generatorService = _createGeneratorService.Invoke(password);
        var masterKey = await generatorService.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = _cryptoServiceFactory.GetCryptoService(masterKey);

        _logger?.LogInformation("Checking login token");
        var result = _tokenService.VerifyToken(cryptoService);

        if(result != TokenResult.Success) {
            _logger?.LogInformation("Login was unsuccessful. Result: {ErrorCode}", result);
            return result;
        }

        _logger?.LogInformation("Login was successful.");
        return Option.Some<ICryptoAccountService, TokenResult>(CreateCryptoAccountAsync(generatorService));

    }

    async Task<ICryptoAccountService> ICryptoAccountServiceFactory.BuildCryptoAccountService(char[] password) {
        KeyGeneratorService generatorService = _createGeneratorService.Invoke(password);
        _ = await generatorService.GenerateKeyAsync().ConfigureAwait(false); //while not used, the token key must be generated to obtain consistent results
        return CreateCryptoAccountAsync(generatorService);
    }


    private ICryptoAccountService CreateCryptoAccountAsync(KeyGeneratorService generator) {
        var lockerReference = new RefCount<Locker>(new());
        return new CryptoAccountService(GenerateKeyAndDisposeGenerator(generator, lockerReference), GenerateKeyAndDisposeGenerator(generator, lockerReference));
    }

    private async Task<Key> GenerateKeyAndDisposeGenerator(KeyGeneratorService generator, RefCount<Locker> lockerRef) {

        try {
            using var reference = lockerRef.GetRef();
            using var lockhere = await reference.Value.GetLockAsync().ConfigureAwait(false);
            return await generator.GenerateKeyAsync().ConfigureAwait(false);
        } finally {
            if(!lockerRef.IsInUse) {
                _logger?.LogInformation("Disposing of KeyGeneratorService...");
                await generator.DisposeAsync().ConfigureAwait(false);
                _logger?.LogInformation("Disposed KeyGeneratorService.");
            }
        }
    }
}