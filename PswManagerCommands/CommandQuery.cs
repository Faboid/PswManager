using PswManagerLibrary.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands {
    public class CommandQuery {

        readonly IReadOnlyCommandsCollection _commands;

        public CommandQuery(IReadOnlyCommandsCollection commands) {
            _commands = commands;
        }

#nullable enable
        public CommandResult Query(string command) {
            //todo - implement a proper parser
            var query = command.Split(' ');
            string cmmType = query.First();
            var args = query.Skip(1).ToArray();

            try {

                return _commands[cmmType].Run(args);

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
