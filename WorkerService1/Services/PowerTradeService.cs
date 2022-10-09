using WorkerService1.Interfaces;

namespace WorkerService1.Services
{
    public class PowerTradeService : BackgroundService
    {
        private readonly ILogger<PowerTradeService> _logger;
        private readonly IConfiguration _appConfiguration;
        private readonly IPowerTradePublisher _powerTradePublisher;
        private readonly IPowerTradeRepository _powerTradeRepository;
        private readonly IFileSystemService _fileSystemService;

        private double _publishingIntervalMinutes;

        public PowerTradeService(
            ILogger<PowerTradeService> logger,
            IConfiguration appConfiguration,
            IPowerTradePublisher powerTradePublisher,
            IPowerTradeRepository powerTradeRepository,
            IFileSystemService fileSystemService)
        {
            _logger = logger;
            _appConfiguration = appConfiguration;
            _powerTradePublisher = powerTradePublisher;
            _powerTradeRepository = powerTradeRepository;
            _fileSystemService = fileSystemService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitialiseService(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Run(async () => await _powerTradeRepository.GetAggregatedPowerTrades())
                        .ContinueWith(async task => await _powerTradePublisher.Publish(task.Result))
                        .ConfigureAwait(false);

                    await Task.Delay(TimeSpan.FromMinutes(_publishingIntervalMinutes));
                }
                catch (Exception exception)
                {
                    _logger.LogCritical(exception, null);
                }
            }
        }

        private void InitialiseService(CancellationToken stoppingToken)
        {
            var hasError = false;

            try
            {
                var publishingDirectory = 
                    _appConfiguration.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}").Value;

                var publishingIntervalString =
                    _appConfiguration.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingIntervalMinutes}").Value;

                if (double.TryParse(publishingIntervalString, out double publishingIntervalMinutes) &&
                    publishingIntervalMinutes > 0)
                {
                    _publishingIntervalMinutes = publishingIntervalMinutes;
                }
                else
                {
                    hasError = true;
                    _logger.LogError(Resources.Resources.LogMessageInvalidPublishInterval);
                }

                if (!string.IsNullOrEmpty(publishingDirectory))
                {
                    if (!_fileSystemService.DirectoryExistis(publishingDirectory))
                    {
                        _fileSystemService.CreateDirectory(publishingDirectory);
                    }
                }
                else
                {
                    hasError = true;
                    _logger.LogError(Resources.Resources.LogMessageInvalidPublishDirectory);
                }
            }
            catch (Exception exception)
            {
                hasError = true;
                _logger.LogCritical(exception, Resources.Resources.LogMessageInitialisationError);
            }
            finally
            {
                if (hasError)
                {
                    throw new TaskCanceledException(
                        Resources.Resources.LogMessageInitialisationError,
                        null,
                        stoppingToken);
                }
            }
        }
    }
}