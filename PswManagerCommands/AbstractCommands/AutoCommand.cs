using PswManagerCommands.Parsing;
using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands.AbstractCommands {

    /// <summary>
    /// [WIP]The children of <see cref="AutoCommand{TCommandInput}"/> create a custom object which will have automatic parsing and validating.
    /// </summary>
    /// <typeparam name="TCommandInput">The type of the custom object created by the children class.</typeparam>
    public abstract class AutoCommand<TCommandInput> : ICommandOld where TCommandInput : new() {

        public AutoCommand() {
            inputValidator = BuildAutoValidator(new AutoValidationBuilder<TCommandInput>());
            parserReady = Parser.CreateInstance().Setup<TCommandInput>();
        }

        private readonly IParserReady parserReady;
        private readonly AutoValidation<TCommandInput> inputValidator;

        public Type GetCommandInputType => typeof(TCommandInput);

        protected abstract AutoValidation<TCommandInput> BuildAutoValidator(AutoValidationBuilder<TCommandInput> builder);
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
