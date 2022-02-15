using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> _commands;

        public CommandQuery(IReadOnlyDictionary<string, ICommand> commands) {
            _commands = commands;
        }

        public Type GetCommandInputTemplate(string commandType) {
            return _commands[commandType].GetCommandInputType; //todo - handle wrong input
        }

        public CommandResult Query(string cmdType, ICommandInput arguments) {

            if(_commands.TryGetValue(cmdType, out var cmd)) {

                return cmd.Run(arguments);
            } else {

                return new CommandResult("The given command doesn't exist. For a list of commands, write \"help\".", false);
            }

        }

    }
}
