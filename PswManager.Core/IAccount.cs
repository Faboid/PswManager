using PswManager.Core.AccountModels;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Core;

/// <summary>
/// Represents an encrypted account. Provides methods for deletion, editing, and decrypting it.
/// </summary>
public interface IAccount : IReadOnlyAccountModel {

    /// <summary>
    /// The name of the account.
    /// </summary>
    new string Name { get; } 

    /// <summary>
    /// The encrypted password of the account.
    /// </summary>
    new string Password { get; }

    /// <summary>
    /// The encrypted email of the account.
    /// </summary>
    new string Email { get; }

    /// <summary>
    /// Deletes the account.
    /// </summary>
    /// <returns></returns>
    Task DeleteAccountAsync();

    /// <summary>
    /// Edits the account with the provided values.
    /// </summary>
    /// <param name="newValues"></param>
    /// <returns></returns>
    Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues);

    /// <summary>
    /// Decrypts the account and returns it.
    /// </summary>
    /// <returns>A decrypted version of the model.</returns>
    DecryptedAccount GetDecryptedModel();

    /// <summary>
    /// Decrypts the account asynchronously and returns it.
    /// </summary>
    /// <remarks>This is implemented as a <see cref="Task.Run{TResult}(System.Func{TResult})"/> wrapper of <see cref="GetDecryptedModel"/>.</remarks>
    /// <returns></returns>
    Task<DecryptedAccount> GetDecryptedModelAsync();
}

public enum EditAccountResult {
    Unknown,
    Success,
    NameCannotBeEmpty,
    PasswordCannotBeEmpty,
    EmailCannotBeEmpty,
    NewNameIsOccupied,
    DoesNotExist,
}