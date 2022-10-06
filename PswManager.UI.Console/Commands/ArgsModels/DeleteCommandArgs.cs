using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.ConsoleUI.Attributes;
using PswManager.ConsoleUI.Commands.Validation.Attributes;

namespace PswManager.ConsoleUI.Commands.ArgsModels;

/// <summary>
/// Represents the arguments used to delete an account.
/// </summary>
public class DeleteCommandArgs : ICommandInput {

    /// <summary>
    /// The name of the account.
    /// </summary>
    [VerifyAccountExistence(true, "The given account doesn't exist.")]
    [Required]
    [Request("Name", "Insert the name of the account you wish to delete.")]
    public string Name { get; set; }

}
