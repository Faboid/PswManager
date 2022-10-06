using PswManager.Commands;
using PswManager.Commands.Validation.Attributes;
using PswManager.UI.Console.Attributes;
using PswManager.UI.Console.Commands.Validation.Attributes;

namespace PswManager.UI.Console.Commands.ArgsModels;

/// <summary>
/// The arguments for the get account command.
/// </summary>
public class GetCommandArgs : ICommandInput {

    /// <summary>
    /// The name of the account.
    /// </summary>
    [VerifyAccountExistence(true, "The given account doesn't exist.")]
    [Required]
    [Request("Name", "Insert the name of the account you wish to get.")]
    public string Name { get; set; }

}
