using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Builders;
using PswManagerCommands.Validation.Models;
using PswManagerDatabase.Config;
using PswManagerLibrary.UIConnection.Attributes;
using System.IO;

namespace PswManagerLibrary.Commands {
    public class MoveDatabaseCommand : BaseCommand<MoveDatabaseCommand.Args> {

        readonly IPaths paths;
        public const string InexistentDirectoryErrorMessage = "The given path must lead to an existing folder.";

        public MoveDatabaseCommand(IPaths paths) {
            this.paths = paths;
        }

        protected override CommandResult RunLogic(Args arguments) {
            paths.MoveMain(arguments.Path);
            return new CommandResult("The database has been moved successfully.", true);
        }

        public override string GetDescription() {
            return "Changes the location where the accounts are stored. The given path must lead to an existing folder.";
        }

        protected override ValidatorBuilder<Args> AddConditions(ValidatorBuilder<Args> builder) => builder
            .AddCondition(new IndexHelper(0, -1), x => Directory.Exists(x.Path), InexistentDirectoryErrorMessage);

        public class Args : ICommandInput {

            [Required]
            [Request("New Database Path", "Please insert a valid path to an empty folder.")]
            public string Path { get; set; }

        }
    }
}
