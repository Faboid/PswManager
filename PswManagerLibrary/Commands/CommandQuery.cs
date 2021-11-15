using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using System;
using System.Linq;

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
                    ThrowIfWrongNumberArguments(2, arguments.Length, "With the command 'psw', it's needed to give two arguments", "[password1] [password2]");

                    return InitializeSetup(arguments[0], arguments[1]);

                case CommandType.Get:
                    ThrowIfWrongNumberArguments(1, arguments.Length, "With the command 'get', it's needed to give one argument:", "[name account]");

                    var account = pswManager.GetPassword(arguments[0]);
                    return account;

                case CommandType.Create:
                    ThrowIfWrongNumberArguments(3, arguments.Length, "With the command 'create', it's needed to give three arguments:", "[name account] [password account] [email account]");

                    pswManager.CreatePassword(arguments[0], arguments[1], arguments[2]);
                    return "A new password has been saved successfully.";

                case CommandType.Edit:
                    //todo - implement editing an account
                    ThrowIfArgumentsOutOfRange(2, 4, arguments.Length, 
                        "With the command 'edit', it's needed to give the account name and at least one optional argument in the following format:", 
                        "[name account] ?[name:new name]? ?[password:new password]? ?[email:new email]?");

                    pswManager.EditPassword(arguments[0], arguments.Skip(1).ToArray());
                    return "temporary end-process message";

                case CommandType.Delete:
                    //todo - implement deleting an account
                    ThrowIfWrongNumberArguments(1, arguments.Length, "With the command 'delete', it's needed to give one argument:", "[name account]");

                    var result = userInput.YesOrNo("Are you sure? This account will be deleted forever.");

                    if(result == false) { return "The operation has been stopped."; }

                    pswManager.DeletePassword(arguments[0]);
                    return "temporary end-process message";

                default:
                    throw new InvalidCommandException(nameof(command.MainCommand), "The given command has found some unknown error.");
            }

        }

        private void ThrowIfWrongNumberArguments(int expected, int actual, string message, string format) {
            if(expected != actual) {
                throw new InvalidCommandException($"Invalid number of arguments.{Environment.NewLine}{message}{Environment.NewLine}{format}");
            }
        }

        private void ThrowIfArgumentsOutOfRange(int min, int max, int actual, string message, string format) {
            if(min < actual || actual > max) {
                throw new InvalidCommandException($"Number of arguments out of range.{Environment.NewLine}{message}{Environment.NewLine}{format}");
            }
        }

    }
}