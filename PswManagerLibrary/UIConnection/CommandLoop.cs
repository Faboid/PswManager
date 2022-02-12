﻿using PswManagerCommands;
using PswManagerDatabase;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Storage;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerLibrary.UIConnection {
    //todo - this class is doing too much. Split it into multiple classes and use proper DI
    public class CommandLoop {

        private IUserInput userInput;
        private CommandQuery query;

        public CommandLoop(IUserInput userInput, ICryptoAccount cryptoAccount, IReadOnlyDictionary<string, ICommand> extraCommands = null) {
            extraCommands ??= new Dictionary<string, ICommand>();

            this.userInput = userInput;
            SetUp(cryptoAccount, extraCommands);
        }

        private void SetUp(ICryptoAccount cryptoAccount, IReadOnlyDictionary<string, ICommand> extraCommands) {

            var dbType = DatabaseType.TextFile;

            IDataFactory dataFactory = new DataFactory(dbType);

            //set up command query
            Dictionary<string, ICommand> collection = new();

            collection.Add("help", new HelpCommand(collection));
            
            //basic crud commands
            collection.Add("add", new AddCommand(dataFactory.GetDataCreator(), cryptoAccount));
            collection.Add("get", new GetCommand(dataFactory.GetDataReader(), cryptoAccount));
            collection.Add("get-all", new GetAllCommand(dataFactory.GetDataReader(), cryptoAccount));
            collection.Add("edit", new EditCommand(dataFactory.GetDataEditor(), cryptoAccount));
            collection.Add("delete", new DeleteCommand(dataFactory.GetDataDeleter(), userInput));

            //database commands
            if(dbType == DatabaseType.TextFile) { 
                collection.Add("movedb", new MoveDatabaseCommand(dataFactory.GetPathsEditor().GetPaths()));
            }

            query = new CommandQuery(collection.Concat(extraCommands).ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// This will keep asking for commands until the user breaks out of it by giving "exit" as the new command
        /// </summary>
        public void Start() {

            string command;
            while((command = userInput.RequestAnswer()).ToLowerInvariant() != "exit") {

                SingleQuery(command);
            }

        }

        public void SingleQuery(string command) {
            var result = query.Query(command);

            userInput.SendMessage(result.BackMessage);
            if(result.QueryReturnValue != null) {
                userInput.SendMessage(result.QueryReturnValue);
            }
            if(result.Success is false && result.ErrorMessages?.Length > 0) {
                var response = userInput.YesOrNo($"There are {result.ErrorMessages.Length} errors. Do you want to read them?");
                if(!response) {
                    return;
                }

                //todo - consider turning this into a foreach and display a single error message at a time
                userInput.SendMessage(result.GetAllErrorsAsSingleString());
            }
        }

    }
}
