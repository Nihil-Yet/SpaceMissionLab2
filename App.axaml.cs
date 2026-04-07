using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpaceMission.Data;
using SpaceMission.Services;
using SpaceMission.ViewModels;
using SpaceMission.Views;
using System;

namespace SpaceMission
{
    public class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            // SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=missions.db"));

            services.AddScoped<IMissionRepository, MissionRepository>();
            services.AddSingleton<IPlanetaryData, EarthData>();

            services.AddTransient<MainWindowViewModel>();

            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
