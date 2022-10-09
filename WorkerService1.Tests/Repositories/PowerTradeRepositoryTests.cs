using System.Collections;
using Microsoft.Extensions.Logging;
using Moq;
using Services;
using WorkerService1.Repositories;
using WorkerService1.Services;

namespace WorkerService1.Tests.Repositories
{
    [TestClass]
    public class PowerTradeRepositoryTests
    {

        private Mock<ILogger<PowerTradeService>> _mockLogger;
        private Mock<IPowerService> _mockPowerService;

        private IList<string> _loggerMessages = new List<string>();

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<PowerTradeService>>();
            _mockPowerService = new Mock<IPowerService>();

            _mockLogger.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback((object o1, object o2, object o3, object o4, object o5) =>
                {
                    var first = ((IEnumerable)o3).OfType<KeyValuePair<string, object>>().FirstOrDefault();
                    _loggerMessages.Add(first.Value.ToString());
                });
        }

        [TestMethod]
        public void PowerTradeRepository_test_power_trades_are_aggrgated_correctly()
        {
            // Arrange
            var powerTrades = CreatePowerTrades();
            _mockPowerService.Setup(ps => ps.GetTradesAsync(It.IsAny<DateTime>()))
                .Returns(Task.FromResult(powerTrades));

            var uut = CreateUnitUnderTest();

            // Act
            var aggregatedPowerTrades = uut.GetAggregatedPowerTrades();

            // Assert
            Assert.IsNotNull(aggregatedPowerTrades.Result);
            Assert.IsTrue(aggregatedPowerTrades.Result.PowerTrades.Count() == 24);
            

            var powerTradeArray = powerTrades.ToArray();
            var aggreatedTradeArray = aggregatedPowerTrades.Result.PowerTrades.ToArray();

            for (var i = 0; i < 24; i++)
            {
                var manualTotal = powerTradeArray[0].Periods[i].Volume + powerTradeArray[1].Periods[i].Volume;

                Assert.AreEqual(manualTotal, aggreatedTradeArray[i].Value);
            }

            Assert.IsTrue(aggreatedTradeArray[0].LocalTime == "23:00");
            Assert.IsTrue(aggreatedTradeArray[1].LocalTime == "00:00");
            Assert.IsTrue(aggreatedTradeArray.Last().LocalTime == "22:00");
        }

        [TestMethod]
        public void PowerTradeRepository_handles_exception_thronw_by_power_service()
        {
            // Arrange
            var testExceptionMessage = "Power Service Exception";
            _mockPowerService.Setup(ps => ps.GetTradesAsync(It.IsAny<DateTime>()))
                .Throws(new PowerServiceException(testExceptionMessage));

            var uut = CreateUnitUnderTest();

            // Act
            var aggregatedPowerTrades = uut.GetAggregatedPowerTrades();

            // Assert
            Assert.IsTrue(_loggerMessages.Any(m => m == Resources.Resources.LogMessagePowerServiceError));
        }

        private PowerTradeRepository CreateUnitUnderTest()
        {
            return new PowerTradeRepository(
                _mockLogger.Object,
                _mockPowerService.Object);
        }

        private IEnumerable<PowerTrade> CreatePowerTrades()
        {
            var random = new Random();
            var dateTime = new DateTime(2022, 10, 09, 13, 22, 0);
            return Enumerable.Range(1, 2).Select(n => PowerTrade.Create(dateTime, 24))
                .Select(pt =>
                {
                    Array.ForEach(
                        pt.Periods.ToArray(),
                        per => per.Volume = Math.Round(random.Next(1000) + random.NextDouble(), 4));

                    return pt;
                }).ToArray();
        }
    }
}