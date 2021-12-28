using System;

namespace PswManagerLibrary.Exceptions {
    public class CommandArgumentsOutOfRangeException : InvalidCommandException {
        public CommandArgumentsOutOfRangeException(string message) : base(message) {
        }

        public CommandArgumentsOutOfRangeException(string command, string message) : base(command, message) {
        }

        public CommandArgumentsOutOfRangeException(string command, string message, Exception innerException) : base(command, message, innerException) {
        }
    }
}
