using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManagerCommands {
    public class CommandQuery {

        readonly IReadOnlyDictionary<string, ICommand> _commands;

        public CommandQuery(IReadOnlyDictionary<string, ICommand> commands) {
            _commands = commands;
        }

        private static readonly CommandResult doesNotExistResult = new("The given command doesn't exist. For a list of commands, write \"help\".", false);

        public (bool success, Type inputType) TryGetCommandInputTemplate(string commandType) {
            if(_commands.TryGetValue(commandType, out var cmd)) {
                return (true, cmd.GetCommandInputType);
            }

            return (false, null);
        }

        public CommandResult Query(string cmdType, ICommandInput arguments) {

            if(_commands.TryGetValue(cmdType, out var cmd)) {

                return cmd.Run(arguments);
            } else {

                return doesNotExistResult;
            }

        }

        public async ValueTask<CommandResult> QueryAsync(string cmdType, ICommandInput arguments) {
            if(_commands.TryGetValue(cmdType, out var cmd)) {

                return await cmd.RunAsync(arguments).ConfigureAwait(false);
            } else {

                return doesNotExistResult;
            }
        }

    }
}
