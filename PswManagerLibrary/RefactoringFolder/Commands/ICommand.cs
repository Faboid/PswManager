using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public interface ICommand {

        CommandResult Run(string[] arguments);

        (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments);

        string GetSyntax();

    }
}
