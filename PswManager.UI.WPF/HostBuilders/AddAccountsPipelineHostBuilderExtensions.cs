using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.Core;
using PswManager.Core.AccountModels;
using PswManager.Core.Services;
using PswManager.Core.Validators;
using PswManager.Database;
using PswManager.Paths;
using PswManager.UI.WPF.Services;
using System;

namespace PswManager.UI.WPF.HostBuilders;

public static class AddAccountsPipelineHostBuilderExtensions {

    public static IHostBuilder AddAccountsPipeline(this IHostBuilder hostBuilder, DatabaseType dbType) {
        return hostBuilder.ConfigureServices(services => {

            services.AddSingleton<IDataFactory, DataFactory>(s => new(dbType));
            services.AddSingleton(s => s.GetRequiredService<IDataFactory>().GetDataConnection());
            services.AddSingleton<IAccountValidator, AccountValidator>();

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