using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.UI.WPF.Stores;

namespace PswManager.UI.WPF.HostBuilders;

/// <summary>
/// Provides extension methods to inject stores.
/// </summary>
public static class AddStoresHostBuilderExtensions {

    /// <summary>
    /// Injects the singleton stores used by this UI.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder AddStores(this IHostBuilder hostBuilder) {

        return hostBuilder.ConfigureServices(services => {
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<AccountsStore>();
        });

    }

}