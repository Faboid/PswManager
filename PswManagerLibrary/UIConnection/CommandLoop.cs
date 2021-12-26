using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.NotImplementedYet;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.UIConnection {
//todo - this class is doing too much. Split it into multiple classes and use proper DI
    public class CommandLoop {

        private IUserInput userInput;
        private CommandQuery query;

        public CommandLoop(IUserInput userInput, IPasswordManager pswManager, IReadOnlyDictionary<string, ICommand> extraCommands = default) {
            this.userInput = userInput;
            SetUp(pswManager, extraCommands);
        }

        private void SetUp(IPasswordManager pswManager, IReadOnlyDictionary<string, ICommand> extraCommands) {

            //set up command query
            Dictionary<string, ICommand> collection = new();

            //basic crud commands
            collection.Add("add", new AddCommand(pswManager));
            collection.Add("get", new GetCommand(pswManager));
            collection.Add("edit", new EditCommand(pswManager));
            collection.Add("delete", new DeleteCommand(pswManager, userInput));

            //todo - implement these commands
            collection.Add("get-all", new GetAllAccountNamesCommand());
            collection.Add("movedb", new ChangeDatabaseLocationCommand());
            collection.Add("help", new HelpCommand());

            query = new CommandQuery((IReadOnlyDictionary<string, ICommand>)collection.Concat(extraCommands));
        }

        /// <summary>
        /// This will keep asking for commands until the user breaks out of it by giving "exit" as the new command
        /// </summary>
        public void Start() {

            string command;
            while((command = userInput.RequestAnswer()).ToLowerInvariant() is not "exit") {

                SingleQuery(command);
            }

        }

        private void SingleQuery(string command) {
            var result = query.Query(command);

            Console.WriteLine(result.BackMessage);
            if(result.QueryReturnValue != null) {
                Console.WriteLine(result.QueryReturnValue);
            }
            if(result.Success is false && result.ErrorMessages?.Length > 0) {
                var response = userInput.YesOrNo($"There are {result.ErrorMessages.Length} errors. Do you want to read them?");
                if(!response) {
                    return;
                }

                //todo - consider turning this into a foreach and display a single error message at a time
                Console.WriteLine(result.GetAllErrorsAsSingleString());
            }
        }

    }
}
