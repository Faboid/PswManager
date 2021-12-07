using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands.Validation {
    public class ConditionValidator {

        public ConditionValidator(Func<string[], bool> validateFunction, string errorMessage) {
            validationFunction = validateFunction;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        private readonly Func<string[], bool> validationFunction;

        public bool Validate(string[] arguments) {
            return validationFunction.Invoke(arguments);
        }

    }
}
