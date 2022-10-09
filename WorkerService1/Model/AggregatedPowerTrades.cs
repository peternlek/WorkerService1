namespace WorkerService1.Model
{
    public class AggregatedPowerTrades
    {
        public DateTime Date { get; set; }
        
        public IEnumerable<AggregatedPowerTrade> PowerTrades { get; set; }
    }
}