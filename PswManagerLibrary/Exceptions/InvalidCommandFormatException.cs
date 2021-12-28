using System;

namespace PswManagerLibrary.Exceptions {
    /// <summary>
    /// An exception to signal user-error regarding a command's format.
    /// </summary>
    public class InvalidCommandFormatException : InvalidCommandException {
        public InvalidCommandFormatException(string message) : base(message) {
        }

        public InvalidCommandFormatException(string command, string message) : base(command, message) {
        }

        public InvalidCommandFormatException(string command, string message, Exception innerException) : base(command, message, innerException) {
        }

    }
}
