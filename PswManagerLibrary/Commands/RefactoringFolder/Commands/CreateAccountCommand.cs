using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public class CreateAccountCommand : BaseCommand {

        public override (string message, string value) Run() {
            if(isValidated == false) {
                if((isValidated = RunValidation().success) == false) {
                    throw new NotImplementedException();
                }
            }

            //implement code
            throw new NotImplementedException();
        }

        public override (bool success, string errorMessage) RunValidation() {
            throw new NotImplementedException();
        }
    }
}
