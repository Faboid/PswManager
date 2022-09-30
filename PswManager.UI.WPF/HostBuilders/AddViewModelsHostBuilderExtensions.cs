using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddSingleton<Func<AccountsListingViewModel>>(s => () => s.GetRequiredService<AccountsListingViewModel>());

            services.AddSingleton<NavigationService<AccountsListingViewModel>>();

            services.AddTransient<SignUpViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AccountsListingViewModel>();


        });
    }


}