using System;
using System.Collections.Generic;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Used as a lookup table to convert a simple word string into an enum that represents a specific command.
    /// </summary>
    public static class CommandTypeAnalyzer {

        private static readonly Dictionary<string, CommandType> dict = new Dictionary<string, CommandType>();

        static CommandTypeAnalyzer() {
            dict.Add("psw", CommandType.Psw);
        }

        public static CommandType Get(string command) {
            if(dict.TryGetValue(command, out CommandType output)) {
                return output;
            } else {
                throw new NotImplementedException();
            }
        }

    }

    public enum CommandType {
        Psw
    }
}
