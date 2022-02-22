using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.TempLocation;
using PswManagerCommands.Validation;
using PswManagerDatabase.Config;
using PswManagerLibrary.UIConnection.Attributes;
using System;
using System.IO;

namespace PswManagerLibrary.Commands {
    public class MoveDatabaseCommand : BaseCommand<MoveDatabaseCommand.PathArgs> {

        readonly IPaths paths;
        public const string InexistentDirectoryErrorMessage = "The given path must lead to an existing folder.";

        public MoveDatabaseCommand(IPaths paths) {
            this.paths = paths;
        }

        public class PathArgs : ICommandInput {

            [Request("New Database Path", "Please insert a valid path to an empty folder.")]
            public string Path { get; set; }

        }

        public override string GetDescription() {
            return "Changes the location where the accounts are stored. The given path must lead to an existing folder.";
        }

        protected override IValidationCollection<PathArgs> AddConditions(IValidationCollection<PathArgs> collection) {
            collection.Add(0, Directory.Exists(collection.GetObject().Path), InexistentDirectoryErrorMessage);

            return collection;
        }

        protected override CommandResult RunLogic(PathArgs arguments) {
            paths.MoveMain(arguments.Path);
            return new CommandResult("The database has been moved successfully.", true);
        }
    }
}
