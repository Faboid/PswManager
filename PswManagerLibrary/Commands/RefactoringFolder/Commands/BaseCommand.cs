using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public abstract class BaseCommand : ICommand {

        protected abstract (string message, string value) RunLogic(string[] arguments);
        protected abstract (bool success, string errorMessage) RunValidation(string[] arguments);

        public (string message, string value) Run(string[] arguments) {
            var result = Validate(arguments);
            result.success.IfFalseThrow(new InvalidCommandException(result.errorMessage));

            return RunLogic(arguments);
        }

        public (bool success, string errorMessage) Validate(string[] arguments) {
            var(isValidated, errorMessage) = RunValidation(arguments);
            return (isValidated, errorMessage);
        }

        public abstract string GetSyntax();

    }
}
