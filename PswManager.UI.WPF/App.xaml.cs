using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PswManager.UI.WPF.HostBuilders;
using PswManager.UI.WPF.Services;
using PswManager.UI.WPF.Stores;
using PswManager.UI.WPF.ViewModels;
using Serilog;
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
					.WriteTo.File("Log.txt", rollingInterval: RollingInterval.Hour); //todo - put a centralized path

			})
			.AddIOAbstractions()
			.AddAccountsPipeline(Database.DatabaseType.InMemory) //todo - use sql db
			.AddUIComponents()
			.AddStores()
			.AddViewModels()
			.AddMainWindow()
			.Build();
	}

	protected override void OnStartup(StartupEventArgs e) {
		_host.Start();

		_host.Services.GetRequiredService<NavigationStore>().CurrentViewModel = _host.Services.GetRequiredService<SignUpViewModel>();

		MainWindow = _host.Services.GetRequiredService<MainWindow>();
		MainWindow.Show();

		base.OnStartup(e);
	}
}
