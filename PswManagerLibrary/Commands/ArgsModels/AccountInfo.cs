using PswManagerCommands;
using PswManagerCommands.Parsing.Attributes;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;

namespace PswManagerLibrary.Commands.AutoCommands.ArgsModels {
    public class AccountInfo : ICommandInput {

        [ParseableKey("name")]
        [Required]
        [VerifyAccountExistence(true)]
        public string Name { get; set; }

        [ParseableKey("pass")]
        [Required]
        public string Password { get; set; }

        [ParseableKey("ema")]
        [Required]
        public string Email { get; set; }

    }
}
