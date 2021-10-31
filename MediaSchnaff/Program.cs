using MediaSchnaff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "MediaSchnaff Discovery Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
        services.AddHttpClient<DiscoveryService>();
    })
    .Build();

// https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service

await host.RunAsync();