using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.UIConnection.Attributes;
using System;
using System.Collections.Generic;

namespace PswManagerLibrary.Commands {
    public class HelpCommand : BaseCommand<HelpCommand.CommandName> {

        private readonly IReadOnlyDictionary<string, ICommand> commands;
        public const string CommandInexistentErrorMessage = "The requested command doesn't exist. For a list of commands, run [help].";

        public HelpCommand(IReadOnlyDictionary<string, ICommand> commands) {
            this.commands = commands;
        }

        protected override CommandResult RunLogic(CommandName arguments) {
            if(string.IsNullOrWhiteSpace(arguments.CmdName)) {
                if(commands.Count == 0) {
                    return new CommandResult("There has been an error: the command list is empty.", false);
                }

                //return generic help message
                string allCommands = $"List Commands:{Environment.NewLine}{string.Join("  ", commands.Keys)}";
                string message = "For help regarding a specific command, write \"help [command]\"";

                return new CommandResult(message, true, allCommands);
            }

            //return command-specific message
            string commandDetails = commands[arguments.CmdName.ToLowerInvariant()].GetDescription();
            return new CommandResult(commandDetails, true);
        }

        public override string GetDescription() {
            return "If it's used without arguments, provides a list of commands. If it gives a command name as an argument, it displays the description and syntax of that command.";
        }

        protected override ValidatorBuilder<CommandName> AddConditions(ValidatorBuilder<CommandName> builder) => builder
            .AddCondition(new IndexHelper(0),
                x => string.IsNullOrWhiteSpace(x.CmdName) || commands.ContainsKey(x.CmdName.ToLowerInvariant()),
                CommandInexistentErrorMessage);

        public class CommandName : ICommandInput {

            [Request("Command Name", "Leave empty to get a list of commands. Insert a command name to get help for a specific command.", true)]
            public string CmdName { get; set; }

        }
    }
}
