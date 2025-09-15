using ErrorLogging;
using Microsoft.Extensions.DependencyInjection;
using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp;

public partial class App : Application {
    public static IServiceProvider? ServiceProvider { get; private set; }

    //  This method initializes the application by configuring services, building a service provider,
    //  and displaying the MainWindow, while handling any exceptions that may occur during startup by
    //  logging an error.
    protected override void OnStartup (StartupEventArgs e){
        try {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to initialise the application OnStartup!");
        }
    }

    //  This method registers application services, while handling any exceptions by logging an error.
    private static void ConfigureServices (IServiceCollection services){
        try {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SortedDictionaryManager>();
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to ConfigureServices!");
        }
    }
}