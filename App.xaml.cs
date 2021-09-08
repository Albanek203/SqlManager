using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SqlManager.Data.Models;
using SqlManager.Data.Repository;
using SqlManager.Data.Service;
using SqlManager.View;

namespace SqlManager {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        public static readonly IServiceProvider ServiceProvider;
        static App() {
            var servicesCollection = new ServiceCollection();
            ConfigServices(servicesCollection);
            ServiceProvider = servicesCollection.BuildServiceProvider();
        }
        private static void ConfigServices(IServiceCollection serviceProvider) {
            serviceProvider.AddSingleton<DataController>();
            serviceProvider.AddTransient<ConnectSettingsWindow>();
            serviceProvider.AddTransient<DbRepository>();
            serviceProvider.AddTransient<DbService>();
            serviceProvider.AddTransient<MainWindow>();
        }
        private void App_OnStartup(object sender, StartupEventArgs e) {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}