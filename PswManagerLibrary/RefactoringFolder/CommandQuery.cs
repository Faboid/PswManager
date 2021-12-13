using PswManagerLibrary.RefactoringFolder.Commands;
using PswManagerLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.RefactoringFolder {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> _commandsDictionary;

        public CommandQuery(IReadOnlyDictionary<string, ICommand> commandsDictionary) {
            _commandsDictionary = commandsDictionary;
        }

#nullable enable
        public CommandResult Query(string command) {
            //todo - implement a proper parser
            var query = command.Split(' ');
            string cmmType = query.First();
            var args = query.Skip(1).ToArray();

            try {

                return _commandsDictionary[cmmType].Run(args);

            } catch(InvalidCommandException ex) {
                //todo - implement a string interpolation that shows both message and correct syntax
                return new CommandResult(ex.Message, false);
            } catch(KeyNotFoundException) {

                return new CommandResult("The given command doesn't exist. For a list of commands, write \"help\".", false);
            }
        }
#nullable disable
    }
}
