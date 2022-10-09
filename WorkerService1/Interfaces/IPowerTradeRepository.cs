using WorkerService1.Model;

namespace WorkerService1.Interfaces
{
    public interface IPowerTradeRepository
    {
        Task<AggregatedPowerTrades> GetAggregatedPowerTrades();
    }
}