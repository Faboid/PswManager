using Microsoft.Extensions.Hosting;
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
			.ConfigureServices(services => {

			}).Build();
	}

	protected override void OnStartup(StartupEventArgs e) {
		_host.Start();
		base.OnStartup(e);
	}
}
