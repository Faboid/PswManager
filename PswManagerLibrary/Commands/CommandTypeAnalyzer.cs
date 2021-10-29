using System;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Used as a lookup table to convert a simple word string into an enum that represents a specific command.
    /// </summary>
    internal class CommandTypeAnalyzer {

        internal static CommandType Get(string command) => command switch {
            "psw" => CommandType.Psw,
            _ => throw new NotImplementedException()
        };

    }

    public enum CommandType {
        Psw
    }
}
