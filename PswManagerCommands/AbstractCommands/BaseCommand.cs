using PswManagerCommands.Parsing;
using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.AbstractCommands {

    /// <summary>
    /// The children of <see cref="BaseCommand"/> deal with parsing and validating manually. 
    /// </summary>
    public abstract class BaseCommand : ICommand {

        //todo - either turn this into abstract and implement it for all commands or somehow use generics to do the same
        public virtual Type GetCommandInputType { get; } = null; 

        protected abstract IValidationCollection AddConditions(IValidationCollection collection);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public CommandResult Run(string[] arguments) {
            var (success, errorMessages) = Validate(arguments);
            if(!success) {
                return new CommandResult("The command has failed the validation process.", false, null, errorMessages.ToArray());
            }

            return RunLogic(arguments);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments) {
            var conditions = AddConditions(new ValidationCollection(arguments)).GetResult();
            var errorMessages = conditions.Where(x => x.condition is false).Select(x => x.errorMessage);
            errorMessages = errorMessages.Concat(ExtraValidation(arguments) ?? Enumerable.Empty<string>());
            return (errorMessages.Any() == false, errorMessages);
        }

        public (bool success, IEnumerable<string> errorMessages) Validate(ICommandInput obj) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="AddConditions"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual IReadOnlyList<string> ExtraValidation(string[] arguments) { return Array.Empty<string>(); }

        protected abstract CommandResult RunLogic(string[] obj);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract string GetSyntax();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract string GetDescription();

    }
}
