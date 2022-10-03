using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PswManager.Core;
using PswManager.Core.AccountModels;
using PswManager.UI.WPF.Commands;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using System;
using System.Windows;

namespace PswManager.UI.WPF.HostBuilders;

public static class AddViewModelsHostBuilderExtensions {

    public static IHostBuilder AddMainWindow(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddTransient<Func<Window, MainViewModel>>((s) => (win) => new MainViewModel(s.GetRequiredService<NavigationStore>(), win, s.GetRequiredService<INotificationService>()));

            services.AddSingleton((s) => {
                var window = new MainWindow();
                window.DataContext = s.GetRequiredService<Func<Window, MainViewModel>>().Invoke(window);
                return window;
            });

        });
    }


    public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<Func<string, DeleteAccountCommand>>(s => name => {
                return new DeleteAccountCommand(name, s.GetRequiredService<AccountsStore>(), s.GetRequiredService<INotificationService>(), s.GetRequiredService<ILoggerFactory>());
            });

            services.AddSingleton<Func<CreateAccountViewModel>>(s => () => s.GetRequiredService<CreateAccountViewModel>());
            services.AddSingleton<Func<AccountsListingViewModel>>(s => () => s.GetRequiredService<AccountsListingViewModel>());
            services.AddSingleton<Func<IAccount, AccountViewModel>>(s => {
                return account => new AccountViewModel(
                    account, s.GetRequiredService<IAccountModelFactory>(), 
                    s.GetRequiredService<Func<string, DeleteAccountCommand>>(),
                    s.GetRequiredService<NavigationService<EditAccountViewModel, DecryptedAccount>>());
            });

            services.AddSingleton<Func<DecryptedAccount, EditAccountViewModel>>(s => {
                return account => new EditAccountViewModel(
                    account, s.GetRequiredService<AccountsStore>(), s.GetRequiredService<IAccountModelFactory>(),
                    s.GetRequiredService<INotificationService>(), s.GetRequiredService<NavigationService<AccountsListingViewModel>>(), 
                    s.GetRequiredService<ILoggerFactory>());
            });

            services.AddSingleton<NavigationService<AccountsListingViewModel>>();
            services.AddSingleton<NavigationService<CreateAccountViewModel>>();
            services.AddSingleton<NavigationService<EditAccountViewModel, DecryptedAccount>>();

            services.AddTransient<SignUpViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AccountsListingViewModel>();
            services.AddTransient<CreateAccountViewModel>();

        });
    }


}