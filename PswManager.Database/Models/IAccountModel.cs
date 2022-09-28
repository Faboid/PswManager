namespace PswManager.Database.Models;

/// <summary>
/// Represents an account.
/// </summary>
public interface IAccountModel : IReadOnlyAccountModel {

    /// <summary>
    /// The account name.
    /// </summary>
    new string Name { get; set; }

    /// <summary>
    /// The account password.
    /// </summary>
    new string Password { get; set; }

    /// <summary>
    /// The account email.
    /// </summary>
    new string Email { get; set; }

}