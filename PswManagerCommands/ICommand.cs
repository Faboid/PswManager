using PswManagerCommands.Parsing;
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
        /// Returns an <see cref="IParserReady"/> to parse the arguments from string to the <see cref="TArgumentsObject"/> required by the specific command.
        /// </summary>
        IParserReady GetParser();

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
