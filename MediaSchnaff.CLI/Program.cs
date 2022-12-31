﻿using MediaSchnaff.Shared.DBAccess;
using MediaSchnaff.Shared.LocalData;
using MediaSchnaff.Shared.Scanning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace MediaSchnaff.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = AppStartup();

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var discovery = host.Services.GetRequiredService<IDiscovery>();

            //if (Debugger.IsAttached)
            //{
            //    //discovery.GetFileInfo(@"V:\Upload\Alex-iPhone\2020\10\2020-10-03_12-24-14_IMG_2187.MOV");
            //    discovery.TranscodeToH264Mp4(@"V:\Upload\Diskstation-Bis-2020\2003\2003_04_13 - BBM, Othello, Panorama\MVI_0024.avi", "c:\\temp\\xxx.mp4");
            //    return;
            //}

            discovery.Scan(@"V:\Upload", true, token);
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            // Check the current directory that the application is running on 
            // Then once the file 'appsetting.json' is found, we are adding it.
            // We add env variables, which can override the configs in appsettings.json

            var configFile = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (Debugger.IsAttached)
                configFile = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\appsettings.json");

            builder.AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            var configRoot = builder.Build();

            // Specifying the configuration for serilog
            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                            .ReadFrom.Configuration(configRoot) // connect serilog to our configuration folder
                            .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                            .WriteTo.Console() // decide where the logs are going to be shown
                            .CreateLogger(); //initialise the logger

            Log.Logger.Information("Application Starting");

            var host = Host
                .CreateDefaultBuilder() // Initialising the Host 
                .ConfigureServices((context, services) => { // Adding the DI container for configuration

                    services.Configure<ApplicationSettings>(configRoot.GetSection("ApplicationSettings"));
                    services.AddTransient<IDirectories, Directories>();
                    services.AddTransient<IDiscovery, Discovery>();
                    services.AddDbContext<MainContext>();

                })
                .UseSerilog() // Add Serilog
                .Build(); // Build the Host

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MainContext>();
                db.Database.Migrate();
            }

            return host;
        }
    }
}