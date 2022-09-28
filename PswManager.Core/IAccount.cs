using PswManager.Core.AccountModels;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccount : IReadOnlyAccountModel {
    new string Name { get; } //todo - add documentation
    new string Password { get; }
    new string Email { get; }

    Task DeleteAccountAsync();
    Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues);
    DecryptedAccount GetDecryptedModel();
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