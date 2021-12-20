using PswManagerCommands.AbstractCommands.BaseCommandCommands;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands {
    public class CommandsCollection : ICommandsCollection, IReadOnlyCommandsCollection {

        Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public ICommand this[string key] => _commands[key];

        public void Add(string key, ICommand command) {
            _commands.Add(key, command);
        }

        //todo - remove IPasswordManager dependency from the commands
        public void AddDefault(IPasswordManager pswManager) {
            //CRUD commands
            _commands.Add("add", new AddCommand(pswManager));
            _commands.Add("get", new GetCommand(pswManager));
            _commands.Add("edit", new EditCommand(pswManager));
            _commands.Add("delete", new DeleteCommand(pswManager));

            //todo - add a command to change paths 
        }

        public IReadOnlyCommandsCollection AsReadOnly() {
            return this;
        }

    }
}
