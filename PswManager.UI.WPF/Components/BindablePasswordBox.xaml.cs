using System.Windows;
using System.Windows.Controls;

namespace PswManager.UI.WPF.Components;
/// <summary>
/// Interaction logic for BindablePasswordBox.xaml
/// </summary>
public partial class BindablePasswordBox : UserControl {

    public string Password {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox), new PropertyMetadata(string.Empty, PasswordChangedFromSource));

    private static void PasswordChangedFromSource(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if(d is BindablePasswordBox box) {
            box.UpdatePassword();
        }
    }

    public BindablePasswordBox() {
        InitializeComponent();
    }

    private bool _isPasswordChanging = false;

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) {
        _isPasswordChanging = true;
        Password = _passwordBox.Password;
        _isPasswordChanging = false;
    }

    private void UpdatePassword() {
        if(!_isPasswordChanging) {
            _passwordBox.Password = Password;
        }
    }
}
