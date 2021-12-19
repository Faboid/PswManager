using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands.ConcreteCommands {
    public abstract class BaseCommand : ICommand {
        protected abstract IValidationCollection AddConditions(IValidationCollection collection);

        public CommandResult Run(string[] arguments) {
            var (success, errorMessages) = Validate(arguments);
            if(!success) {
                return new CommandResult("The command has failed the validation process.", false, null, errorMessages.ToArray());
            }

            return RunLogic(arguments);
        }

        public (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments) {
            var conditions = AddConditions(new ValidationCollection(arguments)).GetResult();
            var errorMessages = conditions.Where(x => x.condition is false).Select(x => x.errorMessage);
            errorMessages = errorMessages.Concat(ExtraValidation(arguments) ?? Enumerable.Empty<string>());
            return (errorMessages.Any() == false, errorMessages);
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="AddConditions"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual IReadOnlyList<string> ExtraValidation(string[] arguments) { return Array.Empty<string>(); }

        protected abstract CommandResult RunLogic(string[] arguments);

        /// <summary>
        /// Gets a string that shows the syntax used by the command.
        /// </summary>
        /// <returns></returns>
        public abstract string GetSyntax();

    }
}
