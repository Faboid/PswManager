using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.Config;
using System;
using System.IO;

namespace PswManagerLibrary.Commands {
    public class MoveDatabaseCommand : BaseCommand {

        readonly IPaths paths;
        public const string InexistentDirectoryErrorMessage = "The given path must lead to an existing folder.";

        public MoveDatabaseCommand(IPaths paths) {
            this.paths = paths;
        }

        public override string GetDescription() {
            return "Changes the location where the accounts are stored. The given path must lead to an existing folder.";
        }

        public override string GetSyntax() {
            return "moveDB [folder path]";
        }
        //todo fix - this won't work with paths that contain spaces since they'll be turned into multiple arguments
        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(1, 1);
            collection.Add(new IndexHelper(0, collection.NullOrEmptyArgsIndexCondition, collection.CorrectArgsNumberIndexCondition),
                (args) => Directory.Exists(args[0]), InexistentDirectoryErrorMessage);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            paths.MoveMain(arguments[0]);
            return new CommandResult("The database has been moved successfully.", true);
        }
    }
}
