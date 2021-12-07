using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public abstract class BaseCommand : ICommand {
        protected abstract IReadOnlyList<ConditionValidator> GetConditions();

        public (string message, string value) Run(string[] arguments) {
            var (success, errorMessages) = Validate(arguments);
            success.IfFalseThrow(new InvalidCommandException(FormatErrors(errorMessages)));

            return RunLogic(arguments);
        }

        public (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments) {
            var errorMessages = GetConditions().Where(x => x.Validate(arguments)).Select(x => x.ErrorMessage);
            ExtraValidation(arguments);
            return (errorMessages.Any() == false, errorMessages);
        }

        private string FormatErrors(IEnumerable<string> errors) {
            StringBuilder sb = new();
            errors.ForEach(x => sb.AppendLine(x));
            return sb.ToString();
        }

        /// <summary>
        /// In case there is the need to validate something that can't fit in <see cref="GetConditions()"/>, this method can be overridden to add such checks.
        /// </summary>
        /// <param name="arguments"></param>
        protected virtual void ExtraValidation(string[] arguments) { }

        protected abstract (string message, string value) RunLogic(string[] arguments);
        public abstract string GetSyntax();

    }
}
