using PswManager.Core;
using PswManager.Core.AccountModels;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

public class AccountViewModel : ViewModelBase {

    public event Action<AccountViewModel>? ShowDetails;
    public event Action? CloseDetails;

    private readonly NavigationService<EditAccountViewModel, DecryptedAccount> _toEditViewModelNavigationService;
    private readonly IAccountModelFactory _accountModelFactory;
    private readonly IAccount _account;

    public string Name => _account.Name;
    public string EncryptedPassword => _account.Password;
    public string EncryptedEmail => _account.Email;

    private IExtendedAccountModel _extendedAccount;
    public string Password => IsDecrypted? _extendedAccount.Password : "Loading...";
    public string Email => IsDecrypted? _extendedAccount.Email : "Loading...";

    public bool IsDecrypted => !_extendedAccount.IsEncrypted;

    private bool _visibleDetails = false;
    public bool VisibleDetails {
        get { return _visibleDetails; }
        set { SetAndRaise(nameof(VisibleDetails), ref _visibleDetails, value); }
    }

    public ICommand DetailsCommand { get; }
    public ICommand CloseDetailsCommand { get; }
    
    private ICommand _editCommand;
    public ICommand EditCommand { get => _editCommand; private set => SetAndRaise(nameof(EditCommand), ref _editCommand, value); }

    public AccountViewModel(IAccount account, IAccountModelFactory accountModelFactory, NavigationService<EditAccountViewModel, DecryptedAccount> toEditAccountViewModel) {
        _account = account;
        _accountModelFactory = accountModelFactory;
        _toEditViewModelNavigationService = toEditAccountViewModel;
        DetailsCommand = new AsyncRelayCommand(OnShowDetails);
        CloseDetailsCommand = new RelayCommand(OnCloseDetails);
        Reset();
    }

    [MemberNotNull(nameof(_extendedAccount), nameof(_editCommand))]
    private void Reset() {
        _extendedAccount = _accountModelFactory.CreateEncryptedAccount(_account);
        OnPropertyChanged(nameof(Password));
        OnPropertyChanged(nameof(Email));

        EditCommand = new AsyncNavigateCommand<EditAccountViewModel, DecryptedAccount>(() => _extendedAccount.GetDecryptedAccountAsync(), true, _toEditViewModelNavigationService);
    }

    private async Task OnShowDetails() {
        VisibleDetails = true;
        ShowDetails?.Invoke(this);
        _extendedAccount = await _extendedAccount.GetDecryptedAccountAsync();
        OnPropertyChanged(nameof(Password));
        OnPropertyChanged(nameof(Email));
    }

    private void OnCloseDetails() {
        VisibleDetails = false;
        CloseDetails?.Invoke();
        Reset();
    }
}