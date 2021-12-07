using PswManagerLibrary.RefactoringFolder.Commands;
using PswManagerLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> commands;

        public CommandQuery() {
            Dictionary<string, ICommand> temp = new();
            temp.Add("add", new AddCommand());

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
                //todo - implement a string interpolation that shows both message and correct syntax
                return (ex.Message, null);
            } catch(KeyNotFoundException) {

                return ("The given command doesn't exist. For a list of commands, write \"help\".", null);
            }
        }
#nullable disable
    }
}
