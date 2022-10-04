﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
