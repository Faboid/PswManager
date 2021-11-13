using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using System;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Takes in commands and analyze them to call their appropriate function.
    /// </summary>
    public class CommandQuery {

        IUserInput userInput;
        IPaths mainPaths;
        IPasswordManager pswManager;
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

        public string InitializeSetup(string passPassword, string emaPassword) {
            CryptoString passCryptoString = new CryptoString(passPassword);
            CryptoString emaCryptoString = new CryptoString(emaPassword);

            token = new Token(passCryptoString, emaCryptoString, mainPaths, userInput);

            if(token.GetUserConfirmation(out string message) is false) {
                return $"The operation has been canceled. Reason: {message}";
            }

            pswManager = new PasswordManager(mainPaths, passCryptoString, emaCryptoString);
            return "The new passwords have been set up successfully.";
        }

        public string Start(string command) {
            return Start(new Command(command));
        }

        public string Start(Command command) {

            var arguments = command.Arguments;

            //todo - add proper results' returns
            switch(command.MainCommand) {
                case CommandType.Psw:
                    return InitializeSetup(arguments[0], arguments[1]);

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