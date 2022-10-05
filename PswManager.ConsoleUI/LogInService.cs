using PswManager.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI;
public class LogInService {
    
    public LogInService(IUserInput userInput, ITokenService tokenService, ICryptoAccountServiceFactory cryptoAccountServiceFactory) {
        _userInput = userInput;
        _tokenService = tokenService;
        _cryptoAccountServiceFactory = cryptoAccountServiceFactory;
    }

    private readonly IUserInput _userInput;
    private readonly ITokenService _tokenService;
    private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;

    public async Task<ICryptoAccountService> AskUserPasswordsAsync() {
#if DEBUG
        if(_userInput.YesOrNo("You are currently in DEBUG mode. Do you want to use pre-made, fixed passwords?")) {
            return new CryptoAccountService("gheerwiahgkth".ToCharArray(), "ewrgrthrer".ToCharArray());
        }
#endif
        return await RequestPasswords().ConfigureAwait(false);
    }

    public ICryptoAccountService AskUserPasswords() {
#if DEBUG
        if(_userInput.YesOrNo("You are currently in DEBUG mode. Do you want to use pre-made, fixed passwords?")) {
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
        _userInput.SendMessage("First time set up initiated.");

        _userInput.SendMessage("Please insert the master key.");
        _userInput.SendMessage("Suggestion: use an easy-to-remember, long password like a passphrase.");

        char[] password;
        do {
            _userInput.SendMessage("Please insert the master key:");
            password = _userInput.RequestPassword();
        } while(!ValidatePassword(password));

        _userInput.SendMessage("The given password is valid.");
        _userInput.SendMessage("Creating a token to validate the password in the future...");

        var account = await _cryptoAccountServiceFactory.SignUpAccountAsync(password);
        _userInput.SendMessage("Completing first time set up... this might take a few seconds.");

        return account;
    }

    private async Task<ICryptoAccountService> LogIn() {

        ICryptoAccountService cryptoAccount;
        while(true) {
            _userInput.SendMessage("Please insert the master key.");

            var password = _userInput.RequestPassword();

            _userInput.SendMessage("Verifying password...");
            var result = await _cryptoAccountServiceFactory.LogInAccountAsync(password);

            //if the password is correct, exit
            if(result.Result() is Utils.Options.OptionResult.Some) {
                cryptoAccount = result.OrDefault();
                if(cryptoAccount != null) {
                    break;
                }
            }

            //if the password is wrong
            _userInput.SendMessage("The given password is incorrect. Please try again.");
        }

        _userInput.SendMessage("The password is correct.");
        _userInput.SendMessage("The log-in process is starting. It might take a few seconds.");

        return cryptoAccount;
    }

    private bool ValidatePassword(char[] password) {

        if(password == null || password.Length < 20) {
            _userInput.SendMessage("The password must be at least 20 characters long.");
            return false;
        }

        _userInput.SendMessage("Please write the password once more:");
        var pass2 = _userInput.RequestPassword();
        if(!password.SequenceEqual(pass2)) {
            _userInput.SendMessage("The given passwords are different from each other. Beware of typos!");
            return false;
        }

        return true;
    }

}
