using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Windows.Storage;
using Yugen.DJ.Interfaces;
using Yugen.DJ.Services;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ
{
    public class AppContainer
    {
        public static IServiceProvider Services { get; set; }

        public static void ConfigureServices()
        {
            string logFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs\\Yugen.Dj.Log.");

            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Debug()
                   .WriteTo.File(logFilePath, restrictedToMinimumLevel: LogEventLevel.Information)
                   .CreateLogger();

            Log.Debug("Serilog started Debug!");
            Log.Information("Serilog started Information!");
            Log.Warning("Serilog started Warning!");

            Services = new ServiceCollection()
                .AddSingleton<IAudioDeviceService, AudioDeviceService>()
                .AddTransient<IAudioService, AudioService>()
                .AddSingleton<MainViewModel>()
                .AddTransient<VinylViewModel>()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(dispose: true);
                })
                .BuildServiceProvider();
        }
    }
}