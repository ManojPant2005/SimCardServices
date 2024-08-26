namespace JioServicesAPI.Service
{
    public class NotificationService : BackgroundService
    {
        private readonly ILogger<NotificationService> _logger;
        public bool IsRunning { get; private set; }

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
            IsRunning = false;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            IsRunning = true;
            _logger.LogInformation("NotificationService started at: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            _logger.LogInformation("NotificationService stopped at: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sending offer message at: {time}", DateTimeOffset.Now);

                // Your logic to send messages and log to database

                await Task.Delay(60000, stoppingToken); // Run every minute
            }
        }
    }
}
