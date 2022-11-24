using PswManager.Core.AccountModels;
using PswManager.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;

internal class AccountsHandler : IAccountsHandler, IAccountsHandlerExecutable {

    private readonly IDataConnection _dataConnection;
    private IAccountModelFactory _currentModelFactory;

    private IExtendedAccountModel[] _accounts = Array.Empty<IExtendedAccountModel>();

    public AccountsHandler(IDataConnection dataConnection, IAccountModelFactory currentModelFactory) {
        _dataConnection = dataConnection;
        _currentModelFactory = currentModelFactory;
    }

    /// <summary>
    /// Updates the stored <see cref="IAccountModelFactory"/> with the new one.
    /// To be used when the password-changing operation completes successfully.
    /// </summary>
    /// <param name="newModelFactory"></param>
    public void UpdateModelFactory(IAccountModelFactory newModelFactory) {
        _currentModelFactory = newModelFactory;
    }

    /// <summary>
    /// Retrieves all accounts and filters to keep only the valid ones.
    /// </summary>
    /// <returns></returns>
    public async Task<IAccountsHandlerExecutable> SetupAccounts(IAccountModelFactory newFactory) {
        var accountsTask = await _dataConnection
            .EnumerateAccountsAsync()
            .Select(x => x.Or(null))
            .Where(x => x is not null)
            .Select(x => _currentModelFactory.CreateEncryptedAccount(x)) //create with current factory
            .Select(x => Task.Run(() => RebuildAccount(x, newFactory))) //decrypt and re-encrypt with new factory
            .ToArrayAsync();

        _accounts = await Task.WhenAll(accountsTask);
        return this;
    }

    public async Task ExecuteUpdate() {

        var updatingTasks = _accounts.Select(x => _dataConnection.UpdateAccountAsync(x.Name, x));
        await Task.WhenAll(updatingTasks);

    }

    /// <summary>
    /// Gets the decrypted account, then re-encrypts it with the new password.
    /// </summary>
    /// <param name="accountModel"></param>
    /// <param name="newModelFactory"></param>
    /// <returns></returns>
    private static EncryptedAccount RebuildAccount(IExtendedAccountModel accountModel, IAccountModelFactory newModelFactory) {
        var decrypted = accountModel.GetDecryptedAccount();
        var newModel = newModelFactory.CreateDecryptedAccount(decrypted);
        return newModel.GetEncryptedAccount();
    }

}