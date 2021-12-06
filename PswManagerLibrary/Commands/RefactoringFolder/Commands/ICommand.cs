using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public interface ICommand {

#nullable enable
        (string message, string? value) Run(string[] arguments);

        (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments);
#nullable disable

        string GetSyntax();

    }
}
