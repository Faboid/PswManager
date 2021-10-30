using System;
using System.Linq;

namespace PswManagerLibrary.Commands {

    /// <summary>
    /// Takes in commands and analyze them to call their appropriate function.
    /// </summary>
    public class CommandQuery {

        public CommandQuery() {
        }

        public void Start(Command command) {
            switch(command.MainCommand) {
                case CommandType.Psw:
                    throw new NotImplementedException();
            }

        }
    }
}