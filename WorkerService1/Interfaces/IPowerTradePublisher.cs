using WorkerService1.Model;

namespace WorkerService1.Interfaces
{
    public interface IPowerTradePublisher
    {
        Task Publish(AggregatedPowerTrades powerTrades);
    }
}
