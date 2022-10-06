using Microsoft.Extensions.Logging;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// When executes, tries to delete the account with the corresponding account name.
/// </summary>
public class DeleteAccountCommand : AsyncCommandBase {

    private readonly string _accountName;
    private readonly AccountsStore _accountsStore;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeleteAccountCommand>? _logger;


    public DeleteAccountCommand(string accountName, AccountsStore accountsStore, INotificationService notificationService, ILoggerFactory loggerFactory = null) {
        _accountName = accountName;
        _accountsStore = accountsStore;
        _notificationService = notificationService;
        _logger = loggerFactory?.CreateLogger<DeleteAccountCommand>();
    }

    protected override async Task ExecuteAsync(object? parameter) {

        _logger?.LogInformation("Beginning to delete {Name}.", _accountName);
        var result = await _accountsStore.DeleteAccountAsync(_accountName);

        if(result is not DeleteAccountResponse.Success) {
            var message = result switch {
                DeleteAccountResponse.AccountNotFound => $"{_accountName} doesn't exist.",
                DeleteAccountResponse.Failure => $"There has been an error trying to delete {_accountName}.",
                _ => "Account deletion has failed for an unknown reason.",
            };

            _logger?.LogInformation("{Name}'s deletion has failed with the error code of {ErrorCode}", _accountName, result);
            _notificationService.Send(message);
        } else {

            _logger?.LogInformation("{Name} has been deleted successfully.", _accountName);
        }

    }
}