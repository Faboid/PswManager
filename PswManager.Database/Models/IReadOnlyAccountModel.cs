namespace PswManager.Database.Models;

/// <summary>
/// Represents a read-only account.
/// </summary>
public interface IReadOnlyAccountModel {

    /// <summary>
    /// The account name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The account password.
    /// </summary>
    string Password { get; }

    /// <summary>
    /// The account email.
    /// </summary>
    string Email { get; }

}

