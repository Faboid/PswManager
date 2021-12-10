using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public abstract class BaseCommand : ICommand {
        protected abstract IReadOnlyList<(bool condition, string errorMessage)> GetConditions(string[] args);

        public (string message, string value) Run(string[] arguments) {
            var (success, errorMessages) = Validate(arguments);
            success.IfFalseThrow(new InvalidCommandException(FormatErrors(errorMessages)));

            return RunLogic(arguments);
        }

        public (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments) {
            var errorMessages = GetConditions(arguments).Where(x => x.condition is false).Select(x => x.errorMessage);
            errorMessages = errorMessages.Concat(ExtraValidation(arguments) ?? Enumerable.Empty<string>());
            return (errorMessages.Any() == false, errorMessages);
        }

        private string FormatErrors(IEnumerable<string> errors) {
            StringBuilder sb = new();
            errors.ForEach(x => sb.AppendLine(x));
            return sb.ToString();
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="GetConditions"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual IReadOnlyList<string> ExtraValidation(string[] arguments) { return Array.Empty<string>(); }

        protected abstract (string message, string value) RunLogic(string[] arguments);

        /// <summary>
        /// Gets a string that shows the syntax used by the command.
        /// </summary>
        /// <returns></returns>
        public abstract string GetSyntax();

    }
}
