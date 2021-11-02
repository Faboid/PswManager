using PswManagerLibrary.Exceptions;
using System;
using System.Collections.Generic;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Used as a lookup table to convert a simple word string into an enum that represents a specific command.
    /// </summary>
    public static class CommandTypeAnalyzer {

        private static readonly Dictionary<string, CommandType> dict = new Dictionary<string, CommandType>();

        static CommandTypeAnalyzer() {
            
            //commands related to the main password
            dict.Add("psw", CommandType.Psw);
            
            //commands that consists in getting a specific entry's info
            dict.Add("get", CommandType.Get);

            //commands that create a new account object info and save it
            dict.Add("create", CommandType.Create);

            //commands that consist in modifying existing entries
            dict.Add("edit", CommandType.Edit);

            //commands that consists in deleting an existing entry
            dict.Add("delete", CommandType.Delete);
        }

        public static CommandType Get(string command) {
            if(dict.TryGetValue(command, out CommandType output)) {
                return output;
            } else {
                throw new InvalidCommandException(command, $"The command \"{command}\" is invalid.");
            }
        }

    }

    public enum CommandType {
        Psw,
        Get,
        Create,
        Edit,
        Delete
    }
}
