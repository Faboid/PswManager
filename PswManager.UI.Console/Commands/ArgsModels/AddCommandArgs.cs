using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.UI.Console.Attributes;
using PswManager.UI.Console.Commands.Validation.Attributes;

namespace PswManager.UI.Console.Commands.ArgsModels;

/// <summary>
/// Represents the arguments used to create a new account.
/// </summary>
public class AddCommandArgs : ICommandInput {

    /// <summary>
    /// The name of the account.
    /// </summary>
    [VerifyAccountExistence(false, AddCommand.AccountExistsErrorMessage)]
    [Required("name")]
    [Request("Name", "Insert the name of the account.")]
    public string Name { get; set; }

    /// <summary>
    /// The password of the account.
    /// </summary>
    [Required("password")]
    [Request("Password", "Insert password.")]
    public string Password { get; set; }

    /// <summary>
    /// The email of the account.
    /// </summary>
    [Required("email")]
    [Request("Email", "Insert email.")]
    public string Email { get; set; }

}
