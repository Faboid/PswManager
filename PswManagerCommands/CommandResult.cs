using System;
using System.Text;

namespace PswManagerCommands {
#nullable enable
    public class CommandResult {

        /// <summary>
        /// The basic message of completion/failure of the command.
        /// </summary>
        public string BackMessage { get; }

        /// <summary>
        /// The value that may be returned by the command(if the command used doesn't return values as shown below, this will be null). 
        /// Example: "[name] [password] [email]" by using a 'get' command.
        /// </summary>
        public string? QueryReturnValue { get; }

        /// <summary>
        /// Whether the command has succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// An array that contains a list with all error messages.
        /// </summary>
        public string[] ErrorMessages { get; }

        public CommandResult(string backMessage, bool success, string queryReturnValue, params string[] errorMessages) : this(backMessage, success) {
            QueryReturnValue = queryReturnValue;
            ErrorMessages = errorMessages ?? Array.Empty<string>();
        }

        public CommandResult(string backMessage, bool success, string[] errorMessages) : this(backMessage, success) {
            ErrorMessages = errorMessages;
        }

        public CommandResult(string backMessage, bool success, string queryReturnValue) : this(backMessage, success) {
            QueryReturnValue = queryReturnValue;
        }

        public CommandResult(string backMessage, bool success) {
            BackMessage = backMessage;
            Success = success;
            ErrorMessages ??= Array.Empty<string>();
        }

        /// <summary>
        /// Returns all the error messages in <see cref="ErrorMessages"/> as a single string. Every error is in its own line, each divided by a line breaker.
        /// </summary>
        /// <returns></returns>
        public string GetAllErrorsAsSingleString() {
            if(ErrorMessages.Length == 0) {
                return "No error messages have been found.";
            }

            StringBuilder sb = new();
            foreach(string s in ErrorMessages) {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }

    }
}
