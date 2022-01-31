using PswManagerHelperMethods;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand<ICommandArgumentsObject>> _commands;

        public CommandQuery(IReadOnlyDictionary<string, ICommand<ICommandArgumentsObject>> commands) {
            _commands = commands;
        }

        public CommandResult Query(string command) {
            string cmdType = command.Split(' ').First().ToLowerInvariant();
            string argsCommand = command.Split(' ').Skip(1).JoinStrings(' ');

            if(_commands.TryGetValue(cmdType, out var cmd)) {
                var parser = cmd.GetParser();
                var args = parser.Parse(argsCommand);

                return cmd.Run(args.Object);
            } else {

                return new CommandResult("The given command doesn't exist. For a list of commands, write \"help\".", false);
            }

        }

    }
}
