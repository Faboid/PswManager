using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Storage {
    public class Token : IToken {

        readonly CryptoAccount cryptoAccount;
        readonly IPaths paths;
        readonly IUserInput userInput;

        public Token(CryptoAccount cryptoAccount, IPaths paths, IUserInput userInput) {
            this.cryptoAccount = cryptoAccount;
            this.paths = paths;
            this.userInput = userInput;
        }

        public bool GetUserConfirmation(out string failureMessage) {
            failureMessage = "";

            //tries to get values
            var result = TryGet();

            //if the values don't exist
            if(result == null) {
                string question = $"The tokens are missing or not set up. Do you want to create new tokens? {Environment.NewLine}" +
                    "Tokens will be used to verify you inserted the correct password(in the future uses). If you refuse to create new, the initialization will be canceled.";

                //if the user wants to set up new tokens
                if(userInput.YesOrNo(question)) {
                    userInput.SendMessage(
                        "You will now insert two tokens: make sure they are easy to remember; it's also suggested to write them somewhere." +
                        Environment.NewLine + "They are the only way to check, in future, if you've inserted the correct passwords."
                    );

                    //yes
                    Set(
                        userInput.RequestAnswer("Write your new first token and press enter. This token will be used to verify your passwords' password."),
                        userInput.RequestAnswer("Write your new second token and press enter. This token will be used to verify your emails' password.")
                    );

                    userInput.SendMessage("The tokens have been set up correctly.");

                    return true;
                } else {
                    //no

                    failureMessage = "To avoid compomising data in the future, it's necessary to set up the tokens.";

                    return false;
                }

            }

            string askTokens = $"The tokens are:{Environment.NewLine} {result.Value.passToken}, {result.Value.emaToken}.{Environment.NewLine} Correct?{Environment.NewLine}" +
                $"Note: if the tokens aren't the ones you've inserted, accepting them as true will give erroneous information and might corrupt the saved data.";

            //if the user recognize the tokens as correct
            if(userInput.YesOrNo(askTokens)) {
                //yes

                return true;

            } else {
                //no

                failureMessage = "Enter correct passwords to obtain the correct tokens.";

                return false;

            }
        }

        public void Set(string passToken, string emaToken) {

            var encrypted = cryptoAccount.Encrypt(passToken, emaToken);
            var tokens = new[] { encrypted.encryptedPassword, encrypted.encryptedEmail };

            File.WriteAllLines(paths.TokenFilePath, tokens);
        }

        /// <summary>
        /// Attempts to get token values. Return null if they don't exist.
        /// </summary>
        public (string passToken, string emaToken)? TryGet() {

            if(File.Exists(paths.TokenFilePath) is false || File.ReadAllText(paths.TokenFilePath) == "") {
                return null;
            }

            var savedToken = File.ReadAllLines(paths.TokenFilePath);

            //decrypt saved tokens
            return cryptoAccount.Decrypt(savedToken[0], savedToken[1]);
        }

        public bool Confront(string passToken, string emaToken) {
            if(File.Exists(paths.TokenFilePath) is false || File.ReadAllText(paths.TokenFilePath) == "") {
                return false;
            }

            //put the tokens in an array to facilitate comparison
            var tokens = ( passToken, emaToken );

            //get encrypted tokens
            var savedToken = File.ReadAllLines(paths.TokenFilePath);

            //decrypt saved tokens
            var savedTokens = cryptoAccount.Decrypt(savedToken[0], savedToken[1]);

            //compare given and decrypted tokens
            return savedTokens == tokens;

        }

    }
}
