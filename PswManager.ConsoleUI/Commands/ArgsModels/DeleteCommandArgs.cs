using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.ConsoleUI.Attributes;
using PswManager.ConsoleUI.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.ArgsModels {
    public class DeleteCommandArgs : ICommandInput {

        [VerifyAccountExistence(true, "The given account doesn't exist.")]
        [Required]
        [Request("Name", "Insert the name of the account you wish to delete.")]
        public string Name { get; set; }

    }
}
