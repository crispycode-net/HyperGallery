using HyperGallery.Shared.DBAccess;
using HyperGallery.Shared.LocalData;
using HyperGallery.Shared.Scanning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace HyperGallery.CLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var host = AppStartup();

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            //var discovery = host.Services.GetRequiredService<IDiscovery>();
            //discovery.Scan(@"V:\Upload", true, token);

            var master = host.Services.GetRequiredService<IScanMaster>();
            await master.ScanAsync(token);

            return 0;
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

                    services.AddDbContext<MainContext>(options => {
                        var connStr = configRoot.GetConnectionString("HyperGalleryConnection");
                        connStr = connStr.Replace("%CommonApplicationData%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                        options.UseSqlServer(connStr);
                        
                    }, ServiceLifetime.Transient);

                    services.AddScoped<IScanMaster, ScanMaster>();

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