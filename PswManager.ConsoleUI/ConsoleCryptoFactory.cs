using PswManager.Async.Locks;
using PswManager.Core;
using PswManager.Core.Services;
using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI;
public class ConsoleCryptoFactory {
    //todo - split/organize this class
    //todo - properly test this class
    public ConsoleCryptoFactory(IUserInput userInput, ITokenService tokenService) {
        this.userInput = userInput;
        _tokenService = tokenService;
    }

    private readonly IUserInput userInput;
    private readonly ITokenService _tokenService;

    public async Task<ICryptoAccountService> AskUserPasswordsAsync() {
#if DEBUG
        if(userInput.YesOrNo("You are currently in DEBUG mode. Do you want to use pre-made, fixed passwords?")) {
            return new CryptoAccountService("gheerwiahgkth".ToCharArray(), "ewrgrthrer".ToCharArray());
        }
#endif
        return await RequestPasswords().ConfigureAwait(false);
    }

    public ICryptoAccountService AskUserPasswords() {
#if DEBUG
        if(userInput.YesOrNo("You are currently in DEBUG mode. Do you want to use pre-made, fixed passwords?")) {
            return new CryptoAccountService("gheerwiahgkth".ToCharArray(), "ewrgrthrer".ToCharArray());
        }
#endif
        //to stop the async methods from propagating to main,
        //they are awaited syncronously in here
        return RequestPasswords()
            .GetAwaiter().GetResult();
    }

    private Task<ICryptoAccountService> RequestPasswords() =>
        _tokenService.IsSet() switch {
            true => LogIn(),
            false => SignUp()
        };

    private async Task<ICryptoAccountService> SignUp() {
        userInput.SendMessage("First time set up initiated.");

        userInput.SendMessage("Please insert the master key.");
        userInput.SendMessage("Suggestion: use an easy-to-remember, long password like a passphrase.");

        KeyGeneratorService generator = null;
        char[] password;
        do {
            userInput.SendMessage("Please insert the master key:");
            password = userInput.RequestPassword();
        } while(!ValidatePassword(password));

        generator = new KeyGeneratorService(password);
        userInput.SendMessage("The given password is valid.");
        userInput.SendMessage("Creating a token to validate the password in the future...");

        var key = await generator.GenerateKeyAsync().ConfigureAwait(false);
        var cryptoService = new CryptoService(key);
        _tokenService.SetToken(cryptoService);

        userInput.SendMessage("Completing first time set up... this might take a few seconds.");

        return CreateCryptoAccountAsync(generator);
    }

    private async Task<ICryptoAccountService> LogIn() {

        KeyGeneratorService generator;
        while(true) {
            userInput.SendMessage("Please insert the master key.");

            var password = userInput.RequestPassword();
            generator = new KeyGeneratorService(password);

            userInput.SendMessage("Verifying password...");
            var key = await generator.GenerateKeyAsync().ConfigureAwait(false);
            var cryptoService = new CryptoService(key);

            //if the password is correct, exit
            if(_tokenService.VerifyToken(cryptoService) == ITokenService.TokenResult.Success) {
                break;
            }

            //since this generator is using a wrong password, it's fine to dispose it
            await generator.DisposeAsync().ConfigureAwait(false);

            //if the password is wrong
            userInput.SendMessage("The given password is incorrect. Please try again.");
        }

        userInput.SendMessage("The password is correct.");
        userInput.SendMessage("The log-in process is starting. It might take a few seconds.");

        return CreateCryptoAccountAsync(generator);
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
                await generator.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private bool ValidatePassword(char[] password) {

        if(password == null || password.Length < 20) {
            userInput.SendMessage("The password must be at least 20 characters long.");
            return false;
        }

        userInput.SendMessage("Please write the password once more:");
        var pass2 = userInput.RequestPassword();
        if(!password.SequenceEqual(pass2)) {
            userInput.SendMessage("The given passwords are different from each other. Beware of typos!");
            return false;
        }

        return true;
    }

}
