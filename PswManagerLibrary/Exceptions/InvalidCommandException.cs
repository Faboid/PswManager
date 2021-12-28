using System;

namespace PswManagerLibrary.Exceptions {
    public class InvalidCommandException : Exception {

        public readonly string InvalidCommand;

        public InvalidCommandException(string message) : base(message) { }

        public InvalidCommandException(string command, string message) : base (message) {
            InvalidCommand = command;
        }

        public InvalidCommandException(string command, string message, Exception innerException) : base (message, innerException) {
            InvalidCommand = command;
        }

    }
}
