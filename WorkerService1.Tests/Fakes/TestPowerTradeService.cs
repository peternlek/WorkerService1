using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkerService1.Interfaces;
using WorkerService1.Services;

namespace WorkerService1.Tests.Fakes
{
    internal class TestPowerTradeService : PowerTradeService
    {
        public TestPowerTradeService(
            ILogger<PowerTradeService> logger,
            IConfiguration appConfiguration,
            IPowerTradePublisher powerTradePublisher,
            IPowerTradeRepository powerTradeRepository,
            IFileSystemService fileSystemService) 
            : base(logger, appConfiguration, powerTradePublisher, powerTradeRepository, fileSystemService)
        {
        }

        public async Task FakeExecuteAsync(int delay)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(async () =>
            {
                base.ExecuteAsync(cancellationTokenSource.Token);
                await Task.Delay(delay);
                throw new TaskCanceledException(
                    "Test Finished",
                    null,
                    cancellationTokenSource.Token);

            }, cancellationTokenSource.Token);
        }
    }
}
