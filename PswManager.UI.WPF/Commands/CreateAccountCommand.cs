using PswManager.Core.AccountModels;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using System.Threading.Tasks;

namespace PswManager.UI.WPF.Commands;

/// <summary>
/// When executed, attempts to create a new account using the values in the properties of the given <see cref="CreateAccountViewModel"/>.
/// </summary>
public class CreateAccountCommand : AsyncCommandBase {

    private readonly IAccountModelFactory _accountModelFactory;
    private readonly CreateAccountViewModel _createAccountViewModel;
    private readonly AccountsStore _accountsStore;
    private readonly INotificationService _notificationService;
    private readonly NavigationService<AccountsListingViewModel> _toAccountsListingViewModel;

    public CreateAccountCommand(CreateAccountViewModel createAccountViewModel, AccountsStore accountsStore, IAccountModelFactory accountModelFactory, INotificationService notificationService, NavigationService<AccountsListingViewModel> toAccountsListingViewModel) {
        _createAccountViewModel = createAccountViewModel;
        _accountModelFactory = accountModelFactory;
        _accountsStore = accountsStore;
        _notificationService = notificationService;
        _toAccountsListingViewModel = toAccountsListingViewModel;
    }

    protected override async Task ExecuteAsync(object? parameter) {

        var model = _accountModelFactory.CreateDecryptedAccount(_createAccountViewModel.AccountName, _createAccountViewModel.Password, _createAccountViewModel.Email);
        var result = await _accountsStore.CreateAccountAsync(model);
        var message = result switch {
            CreateAccountResponse.Success => $"{model.Name} has been created successfully.",
            CreateAccountResponse.Failure => $"The creation of {model.Name} has failed.",
            CreateAccountResponse.NameEmpty => $"{model.Name}'s email cannot be empty.",
            CreateAccountResponse.PasswordEmpty => $"{model.Name}'s password cannot be empty.",
            CreateAccountResponse.EmailEmpty => $"{model.Name}'s email cannot be empty.",
            CreateAccountResponse.NameIsOccupied => $"{model.Name} exists already.",
            _ => $"The creation of {model.Name} has failed for an unknown reason.",
        };

        _notificationService.Send(message);
        
        if(result is CreateAccountResponse.Success) {
            _toAccountsListingViewModel.Navigate(true);
        }

    }
}