using PswManager.Core.AccountModels;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccount {
    string Name { get; }
    string EncryptedPassword { get; }
    string EncryptedEmail { get; }

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