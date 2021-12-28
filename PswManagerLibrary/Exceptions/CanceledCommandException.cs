using System;

namespace PswManagerLibrary.Exceptions {
    public class CanceledCommandException : InvalidCommandException {
        public CanceledCommandException(string message) : base(message) {
        }

        public CanceledCommandException(string command, string message) : base(command, message) {
        }

        public CanceledCommandException(string command, string message, Exception innerException) : base(command, message, innerException) {
        }
    }
}
