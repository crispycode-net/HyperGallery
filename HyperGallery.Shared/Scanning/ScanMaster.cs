using HyperGallery.Shared.LocalData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace HyperGallery.Shared.Scanning
{
    public interface IScanMaster
    {
        Task ScanAsync(CancellationToken cancellationToken);
    }

    public class ScanMaster : IScanMaster
    {
        private readonly IOptions<ApplicationSettings> settings;
        private readonly IServiceProvider serviceProvider;

        public ScanMaster(IOptions<ApplicationSettings> settings, IServiceProvider serviceProvider)
        {
            this.settings = settings;
            this.serviceProvider = serviceProvider;
        }

        public async Task ScanAsync(CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();

            foreach (var item in settings.Value.SourceDirs)
            {
                Task dirTask = ScanDirAsync(item, cancellationToken);
                tasks.Add(dirTask);
            }

            await Task.WhenAll(tasks);
        }

        private Task ScanDirAsync(SourceDir dir, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while(!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(dir.Path))
                        {
                            Log.Warning("Configured source directory {dir} is empty", dir);
                            continue;
                        }

                        if (!Directory.Exists(dir.Path))
                        {
                            Log.Warning("Configured source directory {dir} doesn't exist", dir);
                            continue;
                        }

                        var discoveryService = serviceProvider.GetRequiredService<IDiscovery>();
                        discoveryService.Scan(dir.Path, dir.ScanRecursively, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error while scanning directory {dir}: {msg}", dir, ex.Message);
                    }

                    Task.Delay(10000);
                }

            });
        }
    }
}
