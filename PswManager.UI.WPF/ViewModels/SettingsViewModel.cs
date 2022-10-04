using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using System.Windows.Input;

namespace PswManager.UI.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase {

    public ICommand HomeButton { get; }
	public SettingsViewModel(NavigationService<AccountsListingViewModel> navigationServiceToListingViewModel) {
		HomeButton = new NavigateCommand<AccountsListingViewModel>(true, navigationServiceToListingViewModel);
	}

}