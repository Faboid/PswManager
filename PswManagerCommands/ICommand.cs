using System.Collections.Generic;

namespace PswManagerCommands {
    public interface ICommand {

        CommandResult Run(string[] arguments);

        (bool success, IEnumerable<string> errorMessages) Validate(string[] arguments);

        string[] ParseCommand(string command);

        string GetSyntax();
        string GetDescription();

    }
}
