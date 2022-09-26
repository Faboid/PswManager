using PswManager.Core.AccountModels;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccount {
    Task DeleteAccountAsync();
    Task<EditAccountResult> EditAccountAsync(IAccountModel newValues);
    DecryptedAccount GetDecryptedModel();
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