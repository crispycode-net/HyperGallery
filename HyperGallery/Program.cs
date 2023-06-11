using HyperGallery;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "HyperGallery Discovery Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

// https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service

await host.RunAsync();