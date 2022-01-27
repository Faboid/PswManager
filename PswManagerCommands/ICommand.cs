using System.Collections.Generic;

namespace PswManagerCommands {
    public interface ICommand {

        /// <summary>
        /// Runs the arguments through a validation check, and, if they pass it, runs the command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        CommandResult Run(string[] arguments);

        /// <summary>
        /// Validates the given arguments. If there's any failure, it returns <see langword="false"/> and a list with all the failures that occurred.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments);

        /// <summary>
        /// Parses a command string into a series of arguments. Do not pass in the command type.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string[] ParseCommand(string command);

        /// <summary>
        /// Gets a string that shows the syntax used by the command.
        /// </summary>
        string GetSyntax();

        /// <summary>
        /// Gets a string that describe the command in an user-friendly way.
        /// </summary>
        string GetDescription();

    }
}
