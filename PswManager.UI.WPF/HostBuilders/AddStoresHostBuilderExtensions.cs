using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.UI.WPF.Stores;

namespace PswManager.UI.WPF.HostBuilders;

public static class AddStoresHostBuilderExtensions {

    public static IHostBuilder AddStores(this IHostBuilder hostBuilder) {

        return hostBuilder.ConfigureServices(services => {
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<AccountsStore>();
        });

    }

}