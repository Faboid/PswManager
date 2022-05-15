using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerLibrary.UIConnection.Attributes;

namespace PswManagerLibrary.Commands.AutoCommands.ArgsModels {
    public class EditCommandArgs : ICommandInput {

        [Required]
        [VerifyAccountExistence(true, "The given account doesn't exist.")]
        [Request("Name", "Insert the name of the account.")]
        public string Name { get; set; }

        [VerifyAccountExistence(false, "The given new account name exists already.")]
        [Request("New Name", "If you wish to change the account name, write the new one. If not, leave it empty.", true)]
        public string NewName { get; set; }

        [Request("New Password", "If you wish to change the password, write the new one. If not, leave it empty.", true)]
        public string NewPassword { get; set; }

        [Request("New Email", "If you wish to change the email, write the new one. If not, leave it empty.", true)]
        public string NewEmail { get; set; }

    }
}
