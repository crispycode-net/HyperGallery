namespace HyperGallery
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly ILogger<WindowsBackgroundService> _logger;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Todo: Use HyperGallery.Shared.Scanning.IDiscovery to scan for new images and videos in the backgound

                    throw new NotImplementedException();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation cancelled");
                    break;
                }
            }
        }
    }
}