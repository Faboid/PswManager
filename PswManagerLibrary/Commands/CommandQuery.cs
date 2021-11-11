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
        IPasswordManager pssManager;
        IPaths mainPaths;
        IToken token;

        public CommandQuery(IPaths paths, IUserInput userInput) {
            mainPaths = paths;
            this.userInput = userInput;
        }

        public CommandQuery(IPaths paths, IUserInput userInput, IPasswordManager customPasswordManager) {
            mainPaths = paths;
            pssManager = customPasswordManager;
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
                        string question = "The tokens are missing or not set up. Do you want to create new tokens?";
                        
                        if(userInput.YesOrNo(question)) {
                            //yes


                        } else {
                            //no


                        }

                    }

                    string askTokens = $"The tokens are:{Environment.NewLine} {result.Value.passToken}, {result.Value.emaToken}.{Environment.NewLine} Correct?";

                    if(userInput.YesOrNo(askTokens)) {
                        //yes
                        pssManager = new PasswordManager(mainPaths, arguments[0], arguments[1]);
                        return "A new password has been set up successfully.";

                    } else {
                        //no
                        return "The operation has been canceled. Enter your passwords once more to make sure the tokens are correct.";

                    }

                case CommandType.Get:
                    var account = pssManager.GetPassword(arguments[0]);
                    return account;
                case CommandType.Create:
                    pssManager.CreatePassword(arguments[0], arguments[1], arguments[2]);
                    return "A new password has been saved successfully.";
                case CommandType.Edit:
                    pssManager.EditPassword(arguments[0], arguments[1], arguments[2]);
                    return "temporary end-process message";
                case CommandType.Delete:
                    pssManager.DeletePassword(arguments[0], arguments[1]);
                    return "temporary end-process message";
                default:
                    throw new InvalidCommandException(nameof(command.MainCommand), "The given command has found some unknown error.");
            }

        }
    }
}