using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public abstract class BaseCommand : ICommand {

        protected string[] cachedArguments;
        protected bool isValidated;

        public abstract (string message, string value) Run();
        public abstract (bool success, string errorMessage) RunValidation();

        public (bool success, string errorMessage) Validate() {
            string errorMessage;
            (isValidated, errorMessage) = RunValidation();
            return (isValidated, errorMessage);
        }

        public void SetUp(string[] arguments) {
            cachedArguments = arguments;
            isValidated = false;
        }

        public void Clear() {
            cachedArguments = null;
            isValidated = false;
        }
    }
}
