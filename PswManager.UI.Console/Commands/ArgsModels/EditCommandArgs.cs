using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.UI.Console.Attributes;
using PswManager.UI.Console.Commands.Validation.Attributes;

namespace PswManager.UI.Console.Commands.ArgsModels;

/// <summary>
/// Represents the arguments used to edit an account.
/// </summary>
public class EditCommandArgs : ICommandInput {

    /// <summary>
    /// The current name of the account.
    /// </summary>
    [Required]
    [VerifyAccountExistence(true, "The given account doesn't exist.")]
    [Request("Name", "Insert the name of the account.")]
    public string Name { get; set; }

    /// <summary>
    /// The new name of the account.
    /// </summary>
    [VerifyAccountExistence(false, "The given new account name exists already.")]
    [Request("New Name", "If you wish to change the account name, write the new one. If not, leave it empty.", true)]
    public string NewName { get; set; }

    /// <summary>
    /// The new password of the account.
    /// </summary>
    [Request("New Password", "If you wish to change the password, write the new one. If not, leave it empty.", true)]
    public string NewPassword { get; set; }

    /// <summary>
    /// The new email of the account.
    /// </summary>
    [Request("New Email", "If you wish to change the email, write the new one. If not, leave it empty.", true)]
    public string NewEmail { get; set; }

}
