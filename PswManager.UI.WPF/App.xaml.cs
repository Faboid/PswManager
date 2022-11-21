using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Paths;
using PswManager.UI.WPF.HostBuilders;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Windows;

namespace PswManager.UI.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {

	private readonly IHost _host;

	public App() {
		_host = Host.CreateDefaultBuilder()
			.UseSerilog((host, loggerConfiguration) => {
				loggerConfiguration
					.MinimumLevel.Debug()
					.WriteTo.Debug()
					.WriteTo.File(Path.Combine(DefaultPaths.LogsDirectory, "Logs.txt"), rollingInterval: RollingInterval.Hour);

			})
			.AddIOAbstractions()
			.AddAccountsPipeline(Database.DatabaseType.Sql)
			.AddUIComponents()
			.AddStores()
			.AddViewModels()
			.AddMainWindow()
			.Build();

		AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionsLogger;
	}

	private void UnhandledExceptionsLogger(object sender, UnhandledExceptionEventArgs e) {
		var ex = (Exception)e.ExceptionObject;
		var logger = _host.Services.GetRequiredService<ILogger<App>>();
		logger.LogCritical(ex, "There has been an unhandled exception.");
	}

	protected override async void OnStartup(StartupEventArgs e) {
		_host.Start();

		await _host.Services.GetRequiredService<PasswordEditorFactory>().StartupCheckup();
		var tokenService = _host.Services.GetRequiredService<ITokenService>();
		ViewModelBase startingVm;
		if(tokenService.IsSet()) {
			startingVm = _host.Services.GetRequiredService<LoginViewModel>();
		} else {
			startingVm = _host.Services.GetRequiredService<SignUpViewModel>();
		}

		_host.Services.GetRequiredService<NavigationStore>().CurrentViewModel = startingVm;


		MainWindow = _host.Services.GetRequiredService<MainWindow>();
		MainWindow.Show();

		base.OnStartup(e);
	}
}
