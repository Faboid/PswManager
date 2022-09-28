using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Abstractions;

namespace PswManager.UI.WPF.HostBuilders;

public static class AddIOAbstractionsHostBuilderExtensions {

    public static IHostBuilder AddIOAbstractions(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<FileSystem>();
            services.AddSingleton(s => s.GetRequiredService<FileSystem>().DirectoryInfo);
            services.AddSingleton(s => s.GetRequiredService<FileSystem>().FileInfo);

        });
    }

}