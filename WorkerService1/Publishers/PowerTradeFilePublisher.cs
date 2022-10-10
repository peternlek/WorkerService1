using System.Text;
using WorkerService1.Interfaces;
using WorkerService1.Model;
using WorkerService1.Services;

namespace WorkerService1.Publishers
{
    public class PowerTradeFilePublisher : IPowerTradePublisher
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5, 5);
        private readonly ILogger<PowerTradeService> _logger;
        private readonly IFileSystemService _fileSystemService;
        private readonly string _publishDirectory;

        public PowerTradeFilePublisher(
            ILogger<PowerTradeService> logger,
            IConfiguration appConfiguration,
            IFileSystemService fileSystemService)
        {
            _logger = logger;
            _publishDirectory =
                appConfiguration.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}").Value;
            _fileSystemService = fileSystemService;
        }

        public async Task Publish(AggregatedPowerTrades aggregatedPowerTrades)
        {
            _semaphore.Wait();

            if (_semaphore.CurrentCount == 0)
            {
                _logger.LogWarning("Power Trade report publishing jobs queued and delayed.");
            }

            try
            {
                if (aggregatedPowerTrades != null)
                {
                    var reportFileName = $"PowerPosition_{aggregatedPowerTrades.Date.ToString("yyyyMMdd")}_{aggregatedPowerTrades.Date.ToString("HHmm")}.csv";

                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine("Local Time,Volume");

                    Array.ForEach(
                        aggregatedPowerTrades.PowerTrades.ToArray(),
                        apt => stringBuilder.AppendLine($"{apt.LocalTime},{apt.Value}"));

                    await _fileSystemService.WriteAllTextToFile(
                        Path.Join(_publishDirectory, reportFileName),
                        stringBuilder.ToString());
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error publishing Aggregated Power Trades report");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}