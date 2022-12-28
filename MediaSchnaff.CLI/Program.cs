using MediaSchnaff.Shared;
using MediaSchnaff.Shared.DBAccess;
using MediaSchnaff.Shared.LocalData;
using MediaSchnaff.Shared.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MediaSchnaff.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = AppStartup();

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            //var discovery = ActivatorUtilities.CreateInstance<IDiscovery>(host.Services);
            var discovery = host.Services.GetRequiredService<IDiscovery>();

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

            // Specifying the configuration for serilog
            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                            .ReadFrom.Configuration(builder.Build()) // connect serilog to our configuration folder
                            .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                            .WriteTo.Console() // decide where the logs are going to be shown
                            .CreateLogger(); //initialise the logger

            Log.Logger.Information("Application Starting");

            var host = Host
                .CreateDefaultBuilder() // Initialising the Host 
                .ConfigureServices((context, services) => { // Adding the DI container for configuration

                    // Add services here...
                    services.AddTransient<IDirectories, Directories>();
                    services.AddTransient<IDiscovery, Discovery>();
                    services.AddDbContext<MainContext>();

                })
                .UseSerilog() // Add Serilog
                .Build(); // Build the Host

            return host;
        }
    }
}