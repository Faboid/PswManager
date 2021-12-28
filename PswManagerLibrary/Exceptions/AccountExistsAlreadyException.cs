using System;

namespace PswManagerLibrary.Exceptions {
    public class AccountExistsAlreadyException : InvalidCommandException {
        public AccountExistsAlreadyException(string message) : base(message) {
        }

        public AccountExistsAlreadyException(string command, string message) : base(command, message) {
        }

        public AccountExistsAlreadyException(string command, string message, Exception innerException) : base(command, message, innerException) {
        }
    }
}
