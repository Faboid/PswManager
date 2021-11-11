using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PswManagerLibrary.Global;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Storage;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.UIConnection;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Takes in commands and analyze them to call their appropriate function.
    /// </summary>
    public class CommandQuery {

        IUserInput userInput;
        IPasswordManager pswManager;
        IPaths mainPaths;
        IToken token;

        public CommandQuery(IPaths paths, IUserInput userInput) {
            mainPaths = paths;
            this.userInput = userInput;
        }

        public CommandQuery(IPaths paths, IUserInput userInput, IPasswordManager customPasswordManager) {
            mainPaths = paths;
            pswManager = customPasswordManager;
            this.userInput = userInput;
        }

        public string Start(string command) {
            return Start(new Command(command));
        }

        public string Start(Command command) {

            var arguments = command.Arguments;

            //todo - add proper results' returns
            switch(command.MainCommand) {
                case CommandType.Psw:

                    CryptoString passCryptoString = new CryptoString(arguments[0]);
                    CryptoString emaCryptoString = new CryptoString(arguments[1]);

                    token = new Token(passCryptoString, emaCryptoString, mainPaths);
                    var result = token.Get();

                    if(result == null) {
                        string question = $"The tokens are missing or not set up. Do you want to create new tokens? {Environment.NewLine}" +
                            "Tokens will be used to verify you inserted the correct password(in the next uses).";
                        
                        if(userInput.YesOrNo(question)) {
                            userInput.SendMessage(
                                "You will now insert two tokens: make sure they are easy to remember; it's also suggested to write them somewhere." +
                                Environment.NewLine + "They are the only way to check, in future, if you've inserted the correct passwords."
                            );

                            //yes
                            token.Set(
                                userInput.RequestAnswer("Write your new first token and press enter. This token will be used to verify your passwords' password."), 
                                userInput.RequestAnswer("Write your new second token and press enter. This token will be used to verify your emails' password.")
                            );

                            userInput.SendMessage("The tokens have been set up correctly.");

                            pswManager = new PasswordManager(mainPaths, arguments[0], arguments[1]);
                            return "The new passwords have been set up successfully.";

                        } else {
                            //no
                            return "The operation has been canceled. Tokens are needed to set up the passwords.";

                        }

                    }

                    string askTokens = $"The tokens are:{Environment.NewLine} {result.Value.passToken}, {result.Value.emaToken}.{Environment.NewLine} Correct?{Environment.NewLine}" +
                        $"Note: if the tokens aren't the ones you've inserted, accepting them as true will give erroneous information and might corrupt the saved data.";

                    if(userInput.YesOrNo(askTokens)) {
                        //yes
                        pswManager = new PasswordManager(mainPaths, arguments[0], arguments[1]);
                        return "The new passwords have been set up successfully.";

                    } else {
                        //no
                        return "The operation has been canceled. Enter your passwords once more to obtain the correct tokens.";

                    }

                case CommandType.Get:
                    var account = pswManager.GetPassword(arguments[0]);
                    return account;
                case CommandType.Create:
                    pswManager.CreatePassword(arguments[0], arguments[1], arguments[2]);
                    return "A new password has been saved successfully.";
                case CommandType.Edit:
                    pswManager.EditPassword(arguments[0], arguments[1], arguments[2]);
                    return "temporary end-process message";
                case CommandType.Delete:
                    pswManager.DeletePassword(arguments[0], arguments[1]);
                    return "temporary end-process message";
                default:
                    throw new InvalidCommandException(nameof(command.MainCommand), "The given command has found some unknown error.");
            }

        }
    }
}