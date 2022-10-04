using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.UI.WPF.Services;

namespace PswManager.UI.WPF.HostBuilders;

public static class AddUIComponentsHostBuilderExtensions {

    public static IHostBuilder AddUIComponents(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {
            services.AddSingleton<INotificationService, NotificationService>();
        });
    }

}
