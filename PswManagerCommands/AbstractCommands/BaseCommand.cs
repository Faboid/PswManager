using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.AbstractCommands {

    /// <summary>
    /// Represents a command that uses complex objects as input values.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public abstract class BaseCommand<TInput> : ICommand where TInput : ICommandInput, new() {

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Type GetCommandInputType => typeof(TInput);

        protected abstract IValidationCollection<TInput> AddConditions(IValidationCollection<TInput> collection);


        /// <summary>
        /// <inheritdoc/>
        /// <br/>Note: The given input will be cast to <see cref="TInput"/>. You can get <see cref="TInput"/>'s type by calling <see cref="GetCommandInputType"/>.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public CommandResult Run(ICommandInput arguments) {
            TInput input = (TInput)arguments;
            var (success, errorMessages) = Validate(input);
            if(!success) {
                return new CommandResult("The command has failed the validation process.", false, null, errorMessages.ToArray());
            }

            return RunLogic(input);
        }

        /// <summary>
        /// <inheritdoc/>
        /// <br/>Note: The given input will be cast to <see cref="TInput"/>. You can get <see cref="TInput"/>'s type by calling <see cref="GetCommandInputType"/>.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public (bool success, IEnumerable<string> errorMessages) Validate(ICommandInput arguments) {
            TInput input = (TInput)arguments;
            var conditions = AddConditions(new ValidationCollection<TInput>(input)).GetResult();
            var errorMessages = conditions.Where(x => x.condition is false).Select(x => x.errorMessage);
            errorMessages = errorMessages.Concat(ExtraValidation(input) ?? Enumerable.Empty<string>());
            return (errorMessages.Any() == false, errorMessages);
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="AddConditions"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual IReadOnlyList<string> ExtraValidation(TInput obj) { return Array.Empty<string>(); }

        protected abstract CommandResult RunLogic(TInput obj);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract string GetDescription();

    }
}
