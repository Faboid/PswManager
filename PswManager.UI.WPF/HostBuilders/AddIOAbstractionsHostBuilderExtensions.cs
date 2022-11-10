using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.Core.IO;
using System.IO.Abstractions;

namespace PswManager.UI.WPF.HostBuilders;

/// <summary>
/// Provides extension methods to add the IO abstraction.
/// </summary>
public static class AddIOAbstractionsHostBuilderExtensions {

    /// <summary>
    /// Injects the concrete classes to interact with the file system.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder AddIOAbstractions(this IHostBuilder hostBuilder) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<FileSystem>();
            services.AddSingleton(s => s.GetRequiredService<FileSystem>().DirectoryInfo);
            services.AddSingleton(s => s.GetRequiredService<FileSystem>().FileInfo);
            services.AddSingleton<IDirectoryInfoWrapperFactory, DirectoryInfoWrapperFactory>();

        });
    }

}