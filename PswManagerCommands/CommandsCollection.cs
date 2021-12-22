using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands {
    public class CommandsCollection : ICommandsCollection {

        Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public ICommand this[string key] => _commands[key];

        public void Add(string key, ICommand command) {
            _commands.Add(key, command);
        }

        public IReadOnlyDictionary<string, ICommand> AsReadOnly() {
            return _commands;
        }
    }
}
