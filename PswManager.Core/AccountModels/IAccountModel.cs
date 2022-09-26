using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Core.AccountModels;

/// <summary>
/// Represents a model that could be encrypted or decrypted.
/// </summary>
public interface IAccountModel {

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

	/// <summary>
	/// Whether this model's values are encrypted.
	/// </summary>
	bool IsEncrypted { get; }

	/// <summary>
	/// Whether this model's values are not encrypted.
	/// </summary>
	bool IsPlainText { get; }

	/// <summary>
	/// Gets a database model with the corresponding <see cref="Name"/>, <see cref="Password"/>, and <see cref="Email"/>.
	/// </summary>
	/// <returns></returns>
	public AccountModel GetUnderlyingModel();

	/// <summary>
	/// Gets a decrypted account. Will return its instance if they're already decrypted, or will decrypt them before returning.
	/// </summary>
	/// <returns></returns>
	public DecryptedAccount GetDecryptedAccount();

	/// <summary>
	/// Gets an encrypted account. Will return its instance if they're already encrypted, or will encrypt them before returning.
	/// </summary>
	/// <returns></returns>
	public EncryptedAccount GetEncryptedAccount();

    /// <summary>
    /// Gets a decrypted account asynchronously. Will return its instance if they're already decrypted, or will decrypt them before returning.
    /// </summary>
    /// <returns></returns>
	/// <remarks>The decryption is async because of a Task.Run() wrapper.</remarks>
    public Task<DecryptedAccount> GetDecryptedAccountAsync();

    /// <summary>
    /// Gets an encrypted account asynchronously. Will return its instance if they're already encrypted, or will encrypt them before returning.
    /// </summary>
    /// <returns></returns>
    /// <remarks>The encryption is async because of a Task.Run() wrapper.</remarks>
    public Task<EncryptedAccount> GetEncryptedAccountAsync();

}
