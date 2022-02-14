using PswManagerCommands;
using PswManagerCommands.Parsing.Attributes;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.InputBuilder.Attributes;

namespace PswManagerLibrary.Commands.AutoCommands.ArgsModels {
    public class AccountInfo : ICommandInput {

        [ParseableKey("name")]
        [Required]
        [VerifyAccountExistence(true)]
        [Request("Name", "Please insert the name of the account:")]
        public string Name { get; set; }

        [ParseableKey("pass")]
        [Required]
        [Request("Password", "Please insert a strong password:")]
        public string Password { get; set; }

        [ParseableKey("ema")]
        [Required]
        [Request("Email", "Insert any email:", true)]
        public string Email { get; set; }

    }
}
