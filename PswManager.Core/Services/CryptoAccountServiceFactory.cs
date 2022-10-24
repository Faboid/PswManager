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
    private readonly IKeyGeneratorServiceFactory _keyGeneratorServiceFactory;
    private readonly ITokenService _tokenService;

    public CryptoAccountServiceFactory(ITokenService tokenService, 
                                       ICryptoServiceFactory cryptoServiceFactory, 
                                       IKeyGeneratorServiceFactory keyGeneratorServiceFactory, 
                                       ILoggerFactory? loggerFactory = null) {
        _logger = loggerFactory?.CreateLogger<CryptoAccountServiceFactory>();
        _tokenService = tokenService;
        _cryptoServiceFactory = cryptoServiceFactory;
        _keyGeneratorServiceFactory = keyGeneratorServiceFactory;
    }

    public async Task<ICryptoAccountService> SignUpAccountAsync(char[] password) {

        IKeyGeneratorService generatorService = _keyGeneratorServiceFactory.CreateGeneratorService(password);
        var masterKey = await generatorService.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = _cryptoServiceFactory.GetCryptoService(masterKey);
        _logger?.LogInformation("Setting up new token.");
        _tokenService.SetToken(cryptoService);
        _logger?.LogInformation("A new token has been set up.");
        return CreateCryptoAccountAsync(generatorService);

    }


    public async Task<Option<ICryptoAccountService, TokenResult>> LogInAccountAsync(char[] password) {

        IKeyGeneratorService generatorService = _keyGeneratorServiceFactory.CreateGeneratorService(password);
        var masterKey = await generatorService.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = _cryptoServiceFactory.GetCryptoService(masterKey);

        _logger?.LogInformation("Checking login token");
        var result = _tokenService.VerifyToken(cryptoService);

        if(result != TokenResult.Success) {
            _logger?.LogInformation("Login was unsuccessful. Result: {ErrorCode}", result);
            await generatorService.DisposeAsync();
            return result;
        }

        _logger?.LogInformation("Login was successful.");
        return Option.Some<ICryptoAccountService, TokenResult>(CreateCryptoAccountAsync(generatorService));

    }

    async Task<ICryptoAccountService> ICryptoAccountServiceFactory.BuildCryptoAccountService(char[] password) {
        IKeyGeneratorService generatorService = _keyGeneratorServiceFactory.CreateGeneratorService(password);
        _ = await generatorService.GenerateKeyAsync().ConfigureAwait(false); //while not used, the token key must be generated to obtain consistent results
        return CreateCryptoAccountAsync(generatorService);
    }


    private ICryptoAccountService CreateCryptoAccountAsync(IKeyGeneratorService generator) {
        var lockerReference = new RefCount<Locker>(new());
        var passLock = lockerReference.GetRef();
        var emaLock = lockerReference.GetRef();
        return new CryptoAccountService(GenerateKeyAndDisposeGenerator(generator, lockerReference, passLock), GenerateKeyAndDisposeGenerator(generator, lockerReference, emaLock));
    }

    private async Task<Key> GenerateKeyAndDisposeGenerator(IKeyGeneratorService generator, RefCount<Locker> lockerRef, RefCount<Locker>.Ref thisLock) {

        try {
            using var lockhere = await thisLock.Value.GetLockAsync().ConfigureAwait(false);
            return await generator.GenerateKeyAsync().ConfigureAwait(false);
        } finally {
            
            thisLock.Dispose();

            if(!lockerRef.IsInUse) {
                _logger?.LogInformation("Disposing of KeyGeneratorService...");
                await generator.DisposeAsync().ConfigureAwait(false);
                _logger?.LogInformation("Disposed KeyGeneratorService.");
            }
        }
    }
}