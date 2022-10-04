using PswManager.Core.AccountModels;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Extensions;
using System.Threading.Tasks;

namespace PswManager.Core;

public class CorruptedAccountHolder : IAccountHolder {

    private readonly ReaderErrorCode _readerErrorCode;

    public CorruptedAccountHolder(string name, ReaderErrorCode readerErrorCode, IAccountModelFactory accountModelFactory) {
        _readerErrorCode = readerErrorCode;
        _decryptedAccount = accountModelFactory.CreateDecryptedAccount(name, GetError(name), GetError(name));
    }

    private readonly DecryptedAccount _decryptedAccount;
    public string Name => _decryptedAccount.Name;
    public string Password => _decryptedAccount.Password;
    public string Email => _decryptedAccount.Email;

    public Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues) => EditAccountResult.ValuesCorrupted.AsTask();
    public DecryptedAccount GetDecryptedModel() => _decryptedAccount;
    public Task<DecryptedAccount> GetDecryptedModelAsync() => _decryptedAccount.AsTask();

    private string GetError(string name) => _readerErrorCode switch {
        ReaderErrorCode.UsedElsewhere => $"{name} couldn't be loaded because it was used elsewhere.",
        ReaderErrorCode.DoesNotExist => $"{name} cannot be found.",
        _ => $"There has been an unknown error trying to load {name}",
    };

}