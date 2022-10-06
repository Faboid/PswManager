using Microsoft.Extensions.Logging;
using PswManager.Core;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// Loads all accounts asynchronously, then calls the given action with them as parameter.
/// </summary>
public class LoadAccountsCommand : AsyncCommandBase {

    private readonly ILogger<LoadAccountsCommand>? _logger;
    private readonly AccountsStore _accountsStore;
    private readonly Action<IEnumerable<IAccount>> _loadAccountsMethod;
    private readonly INotificationService _notificationService;

    public LoadAccountsCommand(AccountsStore accountsStore, INotificationService notificationService, Action<IEnumerable<IAccount>> loadAccountsMethod, ILoggerFactory? loggerFactory = null) {
        _accountsStore = accountsStore;
        _loadAccountsMethod = loadAccountsMethod;
        _notificationService = notificationService;
        _logger = loggerFactory?.CreateLogger<LoadAccountsCommand>();
    }

    protected override async Task ExecuteAsync(object? parameter) {
        try {
            await _accountsStore.Load();
            _loadAccountsMethod?.Invoke(_accountsStore.Accounts);
        } catch(Exception ex) {
            _notificationService.Send("Failed to load accounts.");
            _logger?.LogError(ex, "An exception has been thrown while trying to load the accounts.");
        }
    }
}