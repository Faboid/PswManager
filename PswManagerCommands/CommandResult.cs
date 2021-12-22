using System;
using System.Text;

namespace PswManagerCommands {
#nullable enable
    public class CommandResult {

        /// <summary>
        /// The basic message of completion/failure of the message.
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

        public CommandResult(string backMessage, bool success, string? queryReturnValue = null, string[]? errorMessages = null) {
            BackMessage = backMessage;
            QueryReturnValue = queryReturnValue;
            Success = success;
            ErrorMessages = errorMessages ?? Array.Empty<string>();
        }

        /// <summary>
        /// Returns all the error messages in <see cref="ErrorMessages"/> as a single string. Every error is in its own line, each divided by a line breaker.
        /// </summary>
        /// <returns></returns>
        public string GetAllErrorsAsSingleString() {
            StringBuilder sb = new();
            foreach(string s in ErrorMessages) {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }

    }
}
