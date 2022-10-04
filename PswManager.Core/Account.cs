using Microsoft.Extensions.Logging;
using PswManager.Async.Locks;
using PswManager.Core.AccountModels;
using PswManager.Database;
using System.Threading.Tasks;

namespace PswManager.Core;

internal class Account : IAccount {

    private readonly ILogger<Account>? _logger;
    private readonly IDataConnection _connection;
    private readonly IAccountHolder _accountHolder;
    private readonly Locker _locker = new();

    private bool _exists = true;
    public string Name => _accountHolder.Name;
    public string Password => _accountHolder.Password;
    public string Email => _accountHolder.Email;

    public Account(IAccountHolder accountHolder, IDataConnection connection, ILoggerFactory? loggerFactory = null) {
        _logger = loggerFactory?.CreateLogger<Account>();
        _connection = connection;
        _accountHolder = accountHolder;
    }

    public DecryptedAccount GetDecryptedModel() => _accountHolder.GetDecryptedModel();
    public Task<DecryptedAccount> GetDecryptedModelAsync() => _accountHolder.GetDecryptedModelAsync();

    public async Task<EditAccountResult> EditAccountAsync(IExtendedAccountModel newValues) {
        using var locker = await _locker.GetLockAsync();
        if(!_exists) { return EditAccountResult.DoesNotExist; }
        return await _accountHolder.EditAccountAsync(newValues);
    }

    public async Task DeleteAccountAsync() {

        using var locker = await _locker.GetLockAsync();
        _logger?.LogInformation("Starting to delete {Name}.", Name);
        _ = await _connection.DeleteAccountAsync(Name);
        _logger?.LogInformation("{Name} has been deleted.", Name);
        _exists = false;

    }

}