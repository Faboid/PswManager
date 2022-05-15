using PswManager.Commands;
using PswManager.Database;
using PswManager.Core.Commands;
using PswManager.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.UIConnection {
    //todo - this class is doing too much. Split it into multiple classes and use proper DI
    public class CommandLoop {

        private readonly Dictionary<Type, Requester> cachedRequesters = new();
        private readonly IUserInput userInput;
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

            query = new CommandQuery(collection.Concat(extraCommands).ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// This will keep asking for commands until the user breaks out of it by giving "exit" as the new command
        /// </summary>
        public void Start() {

            string cmdType;
            while((cmdType = userInput.RequestAnswer().ToLowerInvariant()) != "exit") {

                if(!TryGetInput(cmdType, out string errorMessage, out var obj)) {
                    userInput.SendMessage(errorMessage);
                    continue;
                }

                SingleQuery(cmdType, (ICommandInput)obj);
            }
        }

        public async Task StartAsync() {
            string cmdType;
            while((cmdType = userInput.RequestAnswer().ToLowerInvariant()) != "exit") {

                if(!TryGetInput(cmdType, out string errorMessage, out var obj)) {
                    userInput.SendMessage(errorMessage);
                    continue;
                }

                await SingleQueryAsync(cmdType, (ICommandInput)obj);
            }
        }

        private bool TryGetInput(string cmdType, out string errorMessage, out object inputObj) {
            
            var (foundTemplate, type) = query.TryGetCommandInputTemplate(cmdType);
            if(!foundTemplate) {
                errorMessage = "The command you've given doesn't exist.";
                inputObj = default;
                return false;
            }

            if(!cachedRequesters.ContainsKey(type)) {
                cachedRequesters.Add(type, new Requester(type, userInput));
            }

            (bool success, inputObj) = cachedRequesters[type].Build(); 
            if(!success) {
                errorMessage = "Something went wrong with the arguments' building phase.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public void SingleQuery(string cmdType, ICommandInput inputArgs) {
            var result = query.Query(cmdType, inputArgs);
            HandleQueryResult(result);
        }

        public async Task SingleQueryAsync(string cmdType, ICommandInput inputArgs) {
            var result = await query.QueryAsync(cmdType, inputArgs);
            HandleQueryResult(result);
        }

        private void HandleQueryResult(CommandResult result) {
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
