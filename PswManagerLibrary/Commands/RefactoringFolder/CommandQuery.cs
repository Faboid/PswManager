using PswManagerLibrary.Commands.RefactoringFolder.Commands;
using PswManagerLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> commands;

        public CommandQuery() {
            Dictionary<string, ICommand> temp = new();
            temp.Add("add", new CreateAccountCommand());

            commands = new ReadOnlyDictionary<string, ICommand>(temp);
        }

#nullable enable
        public (string message, string? value) Query(string command) {
            var query = command.Split(' ');
            string cmmType = query.First();
            var args = query.Skip(1).ToArray();

            try {

                return commands[cmmType].Run(args);

            } catch(InvalidCommandException ex) {
                return (ex.Message, null);
            }
        }
#nullable disable
    }
}
