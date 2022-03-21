using PswManagerCommands;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.UIConnection.Attributes;

namespace PswManagerLibrary.Commands.ArgsModels {
    public class GetCommandArgs : ICommandInput {

        [VerifyAccountExistence(true, "The given account doesn't exist.")]
        [Required]
        [Request("Name", "Insert the name of the account you wish to get.")]
        public string Name { get; set; }

    }
}
