using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;

namespace PswManagerLibrary.Commands {
    public class HelpCommand : BaseCommand {

        private readonly IReadOnlyDictionary<string, ICommand> commands;
        public const string CommandInexistentErrorMessage = "The requested command doesn't exist. For a list of commands, run [help].";

        public HelpCommand(IReadOnlyDictionary<string, ICommand> commands) {
            this.commands = commands;
        }

        public override string GetSyntax() {
            return "help [command]?";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(0, 1);
            if(collection.GetArguments().Length == 1) {
                collection.Add((args) => commands.ContainsKey(args[0]), CommandInexistentErrorMessage);
            }

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            if(arguments.Length == 0) {
                //return generic help message
                string allCommands = $"List Commands:{Environment.NewLine}{string.Join("  ", commands.Keys)}";
                string message = "For help regarding a specific command, write \"help [command]\"";

                return new CommandResult(message, true, allCommands);
            }

            //todo - add method in BaseCommand to request a short explanation from each command
            //return command-specific message
            string commandDetails = "Temporary placeholder for the command details."; //commands[arguments[0]].GetDescription();
            string commandSyntax = commands[arguments[0]].GetSyntax();
            string note =
                commandSyntax.Contains('?') ?
                $"{Environment.NewLine}Command arguments that end with a question mark are optional and can be omitted."
                :
                "";
            return new CommandResult(commandDetails, true, $"{commandSyntax}{note}");
        }
    }
}
