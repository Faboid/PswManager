namespace PswManager.Database.Models;

/// <summary>
/// Represents an account.
/// </summary>
public interface IAccountModel {

    /// <summary>
    /// The account name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// The account password.
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// The account email.
    /// </summary>
    string Email { get; set; }

}