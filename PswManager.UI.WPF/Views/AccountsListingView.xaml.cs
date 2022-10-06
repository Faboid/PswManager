using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PswManager.UI.WPF.Views;
/// <summary>
/// Interaction logic for AccountsListingView.xaml
/// </summary>
public partial class AccountsListingView : UserControl {

    public ICommand LoadAccountsCommand {
        get { return (ICommand)GetValue(LoadAccountsCommandProperty); }
        set { SetValue(LoadAccountsCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LoadAccountsCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LoadAccountsCommandProperty =
        DependencyProperty.Register("LoadAccountsCommand", typeof(ICommand), typeof(AccountsListingView), new PropertyMetadata(null));

    public AccountsListingView() {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        LoadAccountsCommand?.Execute(null);
    }
}
