using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> _commands;

        public CommandQuery(IReadOnlyDictionary<string, ICommand> commands) {
            _commands = commands;
        }

        public CommandResult Query(string command) {
            //todo - implement a proper parser
            var query = command.Split(' ');
            string cmdType = query.First();
            var args = query.Skip(1).ToArray();

            if(_commands.TryGetValue(cmdType, out var cmd)) {

                return cmd.Run(args);
            } else {

                return new CommandResult("The given command doesn't exist. For a list of commands, write \"help\".", false);
            }

        }

    }
}
