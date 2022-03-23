using PswManagerCommands.Validation.Builders;
using PswManagerCommands.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private IValidator<TInput> validator;

        /// <summary>
        /// <inheritdoc/>
        /// <br/>Note: The given input will be cast to <see cref="TInput"/>. You can get <see cref="TInput"/>'s type by calling <see cref="GetCommandInputType"/>.
        /// </summary>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/> is null.</exception>
        /// <returns></returns>
        public CommandResult Run(ICommandInput arguments) {
            TInput input = (TInput)arguments;
            var (success, errorMessages) = Validate(input);
            if(!success) {
                return new CommandResult("The command has failed the validation process.", false, null, errorMessages.ToArray());
            }

            return RunLogic(input);
        }

        protected abstract CommandResult RunLogic(TInput obj);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract string GetDescription();

        /// <summary>
        /// <inheritdoc/>
        /// <br/>Note: The given input will be cast to <see cref="TInput"/>. You can get <see cref="TInput"/>'s type by calling <see cref="GetCommandInputType"/>.
        /// </summary>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/> is null.</exception>
        /// <returns></returns>
        public (bool success, IEnumerable<string> errorMessages) Validate(ICommandInput arguments) {
            if(arguments is null) {
                throw new ArgumentNullException(nameof(arguments), "The given object is null.");
            }

            TInput input = (TInput)arguments;
            var errorMessages = GetValidator().Validate(input);
            errorMessages = errorMessages.Concat(ExtraValidation(input) ?? Enumerable.Empty<string>());
            return (errorMessages.Any() == false, errorMessages);
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="AddConditions"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual IReadOnlyList<string> ExtraValidation(TInput obj) { return Array.Empty<string>(); }
        protected virtual ValidatorBuilder<TInput> AddConditions(ValidatorBuilder<TInput> builder) => builder;
        protected virtual AutoValidatorBuilder<TInput> AddRules(AutoValidatorBuilder<TInput> builder) => builder;

        private IValidator<TInput> GetValidator() {
            if(validator == null) {
                var autoValidator = AddRules(new AutoValidatorBuilder<TInput>()).Build();

                validator = AddConditions(new ValidatorBuilder<TInput>())
                    .AddAutoValidator(autoValidator)
                    .Build();
            }

            return validator;
        }

    }
}
