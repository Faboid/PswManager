using PswManagerCommands;
using PswManagerCommands.Parsing.Attributes;
using PswManagerCommands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.InputBuilder.Attributes;

namespace PswManagerLibrary.Commands.AutoCommands.ArgsModels {
    public class AccountInfo : ICommandInput {

        [Request("Name", "Insert the name of the account.")]
        public string Name { get; set; }

        [Request("Password", "Insert the password.")]
        public string Password { get; set; }

        [Request("Email", "Insert the email.")]
        public string Email { get; set; }

    }
}
