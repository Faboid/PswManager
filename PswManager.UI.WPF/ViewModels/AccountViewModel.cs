using PswManager.Core;

namespace PswManager.UI.WPF.ViewModels;

public class AccountViewModel : ViewModelBase {

    private readonly IAccount _account;

    public string Name => _account.Name;
    public string EncryptedPassword => _account.Password;
    public string EncryptedEmail => _account.Email;

    public AccountViewModel(IAccount account) {
        _account = account;
    }

}