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

        public override string GetDescription() {
            return "If it's used without arguments, provides a list of commands. If it gives a command name as an argument, it displays the description and syntax of that command.";
        }

        public override string GetSyntax() {
            return "help [command]?";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(0, 1);
            if(collection.GetArguments()?.Length == 1) {
                collection.Add((args) => commands.ContainsKey(args[0].ToLowerInvariant()), CommandInexistentErrorMessage);
            }

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            if(arguments.Length == 0) {
                if(commands.Count == 0) {
                    return new CommandResult("There has been an error: the command list is empty.", false);
                }

                //return generic help message
                string allCommands = $"List Commands:{Environment.NewLine}{string.Join("  ", commands.Keys)}";
                string message = "For help regarding a specific command, write \"help [command]\"";

                return new CommandResult(message, true, allCommands);
            }

            arguments[0] = arguments[0].ToLowerInvariant();

            //return command-specific message
            string commandDetails = commands[arguments[0]].GetDescription();
            string commandSyntax = commands[arguments[0]].GetSyntax();
            string note =
                commandSyntax.Contains('?') ?
                $"{Environment.NewLine}Command arguments that end with a question mark are optional and can be omitted."
                :
                "";
            return new CommandResult(commandDetails, true, $"Syntax:{Environment.NewLine}{commandSyntax}{note}");
        }

    }
}
