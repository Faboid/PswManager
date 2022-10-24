using PswManager.Core.AccountModels;
using PswManager.Database;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;

internal class AccountsHandler {

    private readonly IDataConnection _dataConnection;
    private IAccountModelFactory _currentModelFactory;

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
    public async Task<IExtendedAccountModel[]> GetAccounts() {
        return await _dataConnection
            .EnumerateAccountsAsync()
            .Select(x => x.Or(null))
            .Where(x => x is not null)
            .Select(x => _currentModelFactory.CreateEncryptedAccount(x))
            .ToArrayAsync();
    }

    /// <summary>
    /// Deletes all the given accounts.
    /// </summary>
    /// <param name="accounts"></param>
    /// <returns></returns>
    public async Task DeleteAccounts(IExtendedAccountModel[] accounts) {
        var deleteTasks = accounts.Select(x => _dataConnection.DeleteAccountAsync(x.Name));
        await Task.WhenAll(deleteTasks);
    }

    /// <summary>
    /// Creates all the given accounts.
    /// </summary>
    /// <param name="accounts"></param>
    /// <returns></returns>
    public async Task RecreateAccounts(IExtendedAccountModel[] accounts) {
        var recreatingTasks = accounts.Select(x => _dataConnection.CreateAccountAsync(x));
        await Task.WhenAll(recreatingTasks);
    }

    /// <summary>
    /// Decrypts all accounts, then re-encrypts them with the new password.
    /// </summary>
    /// <param name="newFactory"></param>
    /// <returns></returns>
    public async Task<IExtendedAccountModel[]> RebuildAccounts(IExtendedAccountModel[] accounts, IAccountModelFactory newFactory) {
        var accs = accounts
            .AsParallel()
            .Select(x => Task.Run(() => RebuildAccount(x, newFactory)))
            .ToArray();

        return await Task.WhenAll(accs);
    }

    /// <summary>
    /// Gets the decrypted account, then re-encrypts it with the new password.
    /// </summary>
    /// <param name="accountModel"></param>
    /// <param name="newModelFactory"></param>
    /// <returns></returns>
    private EncryptedAccount RebuildAccount(IExtendedAccountModel accountModel, IAccountModelFactory newModelFactory) {
        var decrypted = accountModel.GetDecryptedAccount();
        var newModel = newModelFactory.CreateDecryptedAccount(decrypted);
        return newModel.GetEncryptedAccount();
    }

}