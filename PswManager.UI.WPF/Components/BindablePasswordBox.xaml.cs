using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PswManager.UI.WPF.Components;
/// <summary>
/// Interaction logic for BindablePasswordBox.xaml
/// </summary>
public partial class BindablePasswordBox : UserControl {

    public string Password {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    private const FrameworkPropertyMetadataOptions options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault;
    private const UpdateSourceTrigger updateTrigger = UpdateSourceTrigger.PropertyChanged;

    // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox), new FrameworkPropertyMetadata(string.Empty, options, PasswordChangedFromSource, null, false, updateTrigger));

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
        UpdateErrors();
        _isPasswordChanging = false;
    }

    private void UpdatePassword() {
        if(!_isPasswordChanging) {
            _passwordBox.Password = Password;
        }
    }

    private void UpdateErrors() {
        _errorsItemControl.ItemsSource = Validation.GetErrors(this).Select(x => x.ErrorContent.ToString());
    }

}
