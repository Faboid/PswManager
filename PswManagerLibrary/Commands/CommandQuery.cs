using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PswManagerLibrary.Global;
using PswManagerLibrary.Exceptions;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Takes in commands and analyze them to call their appropriate function.
    /// </summary>
    public class CommandQuery {

        IPasswordManager pssManager;
        IPaths mainPaths;

        public CommandQuery(IPaths paths) {
            mainPaths = paths;
        }

        public CommandQuery(IPaths paths, IPasswordManager customPasswordManager) {
            mainPaths = paths;
            pssManager = customPasswordManager;
        }

        public string Start(Command command) {

            var arguments = command.Arguments;

            //todo - add proper results' returns
            switch(command.MainCommand) {
                case CommandType.Psw:
                    pssManager = new PasswordManager(mainPaths, arguments[0], arguments[1]);
                    return "A new password has been set up successfully.";
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