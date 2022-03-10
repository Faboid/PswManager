using PswManagerCommands;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.UIConnection.Attributes;

namespace PswManagerLibrary.Commands.ArgsModels {
    public class AddCommandArgs : ICommandInput {

        [VerifyAccountExistence(false, AddCommand.AccountExistsErrorMessage)]
        [Required("name")]
        [Request("Name", "Insert the name of the account.")]
        public string Name { get; set; }

        [Required("password")]
        [Request("Password", "Insert password.")]
        public string Password { get; set; }

        [Required("email")]
        [Request("Email", "Insert email.")]
        public string Email { get; set; }

    }
}
