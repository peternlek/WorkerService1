using Services;
using WorkerService1.Interfaces;
using WorkerService1.Model;
using WorkerService1.Services;

namespace WorkerService1.Repositories
{
    public class PowerTradeRepository : IPowerTradeRepository
    {
        private readonly ILogger<PowerTradeService> _logger;
        private readonly IPowerService _powerService;

        public PowerTradeRepository(
            ILogger<PowerTradeService> logger,
            IPowerService powerService)
        {
            _logger = logger;
            _powerService = powerService;
        }

        public async Task<AggregatedPowerTrades> GetAggregatedPowerTrades()
        {
            AggregatedPowerTrades aggregatedPowerTrades = new AggregatedPowerTrades
            {
                Date = DateTime.Now
            };

            try
            {
                aggregatedPowerTrades = await _powerService.GetTradesAsync(aggregatedPowerTrades.Date)
                    .ContinueWith(task =>
                    {
                        var timePeriodCount = -2;
                        aggregatedPowerTrades.PowerTrades = task.Result.SelectMany(pt => pt.Periods).GroupBy(p => p.Period)
                            .Select(grouping =>
                            {
                                var timePeriod = grouping.Key == 1
                                    ? "23:00"
                                    : $"{(timePeriodCount + grouping.Key).ToString().PadLeft(2, '0')}:00";

                                return new AggregatedPowerTrade
                                {
                                    LocalTime = timePeriod, 
                                    Value = grouping.Sum(p => p.Volume),
                                    Order = timePeriodCount
                                };
                            }).ToArray();

                        return aggregatedPowerTrades;
                    });
            }
            catch (AggregateException aggregateException)
            {
                Array.ForEach(
                    aggregateException.InnerExceptions.ToArray(),
                    ex => _logger.LogError(ex, Resources.Resources.LogMessagePowerServiceError));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, Resources.Resources.LogMessagePowerServiceError);
            }

            return aggregatedPowerTrades;
        }
    }
}