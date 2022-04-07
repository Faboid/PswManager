using PswManagerEncryption.Services;
using PswManagerLibrary.UIConnection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PswManagerLibrary.Cryptography {
    public class CryptoFactory {

        public CryptoFactory(IUserInput userInput) {
            this.userInput = userInput;
        }

        private readonly IUserInput userInput;

        public ICryptoAccount AskUserPasswords() {
#if DEBUG
            if(userInput.YesOrNo("You are currently in DEBUG mode. Do you want to use pre-made, fixed passwords?")) {
                return new CryptoAccount("gheerwiahgkth".ToCharArray(), "ewrgrthrer".ToCharArray());
            }
#endif
            //to stop the async methods from propagating to main,
            //they are awaited syncronously in here
            return RequestPasswords().GetAwaiter().GetResult();
        }

        private async Task<ICryptoAccount> RequestPasswords() =>
            Token.IsTokenSetUp(Token.GetDefaultPath()) switch {
                true => await LogIn(),
                false => await SignUp()
            };

        private async Task<ICryptoAccount> SignUp() {
            userInput.SendMessage("First time set up initiated.");

            userInput.SendMessage("Please insert the master key.");
            userInput.SendMessage("Suggestion: use an easy-to-remember, long password like a passphrase.");

            Token token;
            KeyGeneratorService generator;
            do {
                char[] password;
                do {
                    userInput.SendMessage("Please insert the master key:");
                    password = userInput.RequestPassword();
                } while(!ValidatePassword(password));

                //todo - start keys generation before the password is validated
                generator = new KeyGeneratorService(password);
                userInput.SendMessage("The given password is valid.");
                userInput.SendMessage("Creating a token to validate the password in the future...");

                token = new Token(await generator.GenerateKeyAsync());

            } while(!token.VerifyToken());

            userInput.SendMessage("Completing first time set up... this might take a few seconds.");

            try {
                return new CryptoAccount(await generator.GenerateKeyAsync(), await generator.GenerateKeyAsync());
            } finally {
                await generator.DisposeAsync();
            }
        }

        private async Task<ICryptoAccount> LogIn() {

            while(true) {
                userInput.SendMessage("Please insert the master key.");

                var password = userInput.RequestPassword();
                await using var generator = new KeyGeneratorService(password);

                userInput.SendMessage("Verifying password...");
                var token = new Token(await generator.GenerateKeyAsync());
                
                //if the password is correct
                if(token.VerifyToken()) {
                    userInput.SendMessage("The password is correct.");
                    userInput.SendMessage("The log-in process is starting. It might take a few seconds.");

                    return new CryptoAccount(await generator.GenerateKeyAsync(), await generator.GenerateKeyAsync());
                }

                //if the password is wrong
                userInput.SendMessage("The given password is incorrect. Please try again.");
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
}
