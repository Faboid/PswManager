using System;
using System.Collections.Generic;

namespace PswManagerCommands {
    public interface ICommandOld {

        public Type GetCommandInputType { get; }

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
        /// Gets a string that shows the syntax used by the command.
        /// </summary>
        string GetSyntax();

        /// <summary>
        /// Gets a string that describe the command in an user-friendly way.
        /// </summary>
        string GetDescription();

    }

    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand {

        /// <summary>
        /// Gets the type of input object the command requires as input.
        /// </summary>
        public Type GetCommandInputType { get; }

        /// <summary>
        /// Runs the arguments through a validation check, and, if they pass it, runs the command.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        CommandResult Run(ICommandInput input);

        /// <summary>
        /// Validates the given arguments. If there's any failure, it returns <see langword="false"/> and a list with all the failures that occurred.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        (bool success, IEnumerable<string> errorMessages) Validate(ICommandInput input);

        /// <summary>
        /// Gets a string that describe the command in an user-friendly way.
        /// </summary>
        string GetDescription();

    }
}
