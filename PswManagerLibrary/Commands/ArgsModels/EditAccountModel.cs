using PswManagerCommands;
using PswManagerCommands.Parsing.Attributes;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;

namespace PswManagerLibrary.Commands.AutoCommands.ArgsModels {
    public class EditAccountModel : ICommandInput {

        [ParseableKey("n")]
        [Required]
        [VerifyAccountExistence(true)]
        public string Name { get; set; }

        [ParseableKey("name")]
        public string NewName { get; set; }

        [ParseableKey("pass")]
        public string NewPassword { get; set; }

        [ParseableKey("ema")]
        public string NewEmail { get; set; }

    }
}
