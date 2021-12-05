using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public interface ICommand {

#nullable enable
        (string message, string? value) Run();

        (bool success, string? errorMessage) Validate();
#nullable disable

        /// <summary>
        /// Sets up the arguments to execute the command.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>An instance of itself.</returns>
        void SetUp(string[] arguments);

        void Clear();

    }
}
