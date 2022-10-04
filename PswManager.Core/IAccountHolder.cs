using PswManager.Core.AccountModels;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Core;

/// <summary>
/// Stores an <see cref="EncryptedAccount"/> and provides methods to interact with it.
/// </summary>
internal interface IAccountHolder : IReadOnlyAccountModel {

    /// <summary>
    /// <inheritdoc cref="IAccount.GetDecryptedModel"/>
    /// </summary>
    /// <returns></returns>
    DecryptedAccount GetDecryptedModel();

    /// <summary>
    /// <inheritdoc cref="IAccount.GetDecryptedModelAsync"/>
    /// </summary>
    /// <returns></returns>
    Task<DecryptedAccount> GetDecryptedModelAsync();

    /// <summary>
    /// <inheritdoc cref="IAccount.EditAccountAsync(IExtendedAccountModel)"/>
    /// </summary>
    /// <param name="newValues"></param>
    /// <returns></returns>
    Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues);
}
