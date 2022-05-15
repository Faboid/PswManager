using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.Core.Commands.Validation.Attributes;
using PswManager.Core.UIConnection.Attributes;

namespace PswManager.Core.Commands.ArgsModels {
    public class GetCommandArgs : ICommandInput {

        [VerifyAccountExistence(true, "The given account doesn't exist.")]
        [Required]
        [Request("Name", "Insert the name of the account you wish to get.")]
        public string Name { get; set; }

    }
}
