using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.Core;
using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Encryption.Services;
using PswManager.Paths;
using PswManager.UI.WPF.Services;
using System;

namespace PswManager.UI.WPF.HostBuilders;

/// <summary>
/// Provides extension methods to add the account pipeline to a <see cref="IHostBuilder"/>.
/// </summary>
public static class AddAccountsPipelineHostBuilderExtensions {

    /// <summary>
    /// Injects the backend neeeded to set up all the accounts. This includes their factories, objects, database, and encryption.
    /// </summary>
    /// <remarks>
    /// Supports different types of database initialization.
    /// </remarks>
    /// <param name="hostBuilder"></param>
    /// <param name="dbType">The type of database to use.</param>
    /// <returns></returns>
    public static IHostBuilder AddAccountsPipeline(this IHostBuilder hostBuilder, DatabaseType dbType) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<IDataFactory, DataFactory>(s => new(dbType));
            services.AddSingleton(s => s.GetRequiredService<IDataFactory>().GetDataConnection());
            services.AddSingleton<IAccountValidator, AccountValidator>();

            services.AddSingleton<ICryptoServiceFactory, CryptoServiceFactory>();
            services.AddSingleton<IPathsBuilder, PathsBuilder>();
            services.AddSingleton<TokenServiceFactory>();
            services.AddSingleton(s => s.GetRequiredService<TokenServiceFactory>().CreateTokenService("a99521fb-a649-4066-bba1-22414444d227"));
            services.AddSingleton<ICryptoAccountServiceFactory, CryptoAccountServiceFactory>();

            services.AddSingleton<CryptoContainerService>();
            services.AddSingleton<IAccountModelFactory, AccountModelFactory>(InitializeModelFactory);

            services.AddSingleton<IAccountFactory, AccountFactory>();

        });
    }


    private static AccountModelFactory InitializeModelFactory(IServiceProvider s) {
        var cryptoAccount = s.GetService<CryptoContainerService>()!.CryptoAccountService;
        if(cryptoAccount == null) {
            throw new InvalidOperationException($"Tried to initialize {nameof(AccountModelFactory)} before setting up {nameof(CryptoContainerService.CryptoAccountService)}");
        }

        return new(cryptoAccount);
    }

}