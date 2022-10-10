using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Services;
using WorkerService1.Interfaces;
using WorkerService1.Model;
using WorkerService1.Publishers;
using WorkerService1.Services;

namespace WorkerService1.Tests.Publishers
{
    [TestClass]
    public class PowerTradeFilePublisherTests
    {
        private const string TestPublishDirectory = "TestDirectory";

        private Mock<ILogger<PowerTradeService>> _mockLogger;
        private Mock<IFileSystemService> _mockFileSystemService;
        private Mock<IConfiguration> _mockConfiguration;

        private Mock<IConfigurationSection> _mockPublishDirectoryConfigurationSection;

        private IList<string> _loggerMessages = new List<string>();

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<PowerTradeService>>();
            _mockFileSystemService = new Mock<IFileSystemService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPublishDirectoryConfigurationSection = new Mock<IConfigurationSection>();

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
        public void PowerTradeFilePublisher_test_aggregated_power_trades_are_published()
        {
            // Arrange
            var exprectedReportOutput = "Local Time,Volume\r\n23:00,150.4567\r\n00:00,867.47\r\n";
            var exprectedFileName = @$"{TestPublishDirectory}\PowerPosition_20221009_1322.csv";
            var aggregatedPowertrades = new AggregatedPowerTrades
            {
                Date = new DateTime(2022,10,09,13,22,0),
                PowerTrades = new AggregatedPowerTrade[]
                {
                    new AggregatedPowerTrade
                    {
                        LocalTime = "23:00",
                        Value = 150.4567
                    },
                    new AggregatedPowerTrade
                    {
                        LocalTime = "00:00",
                        Value = 867.47
                    }
                }
            };

            _mockPublishDirectoryConfigurationSection
                .Setup(x => x.Value)
                .Returns(TestPublishDirectory);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}"))
                .Returns(_mockPublishDirectoryConfigurationSection.Object).Verifiable();

            var uut = CreateUnitUnderTest();

            // Act
            uut.Publish(aggregatedPowertrades);

            // Assert
            _mockConfiguration.VerifyAll();
            _mockFileSystemService.Verify(fss => 
                fss.WriteAllTextToFile(It.Is<string>(s => exprectedFileName == s), It.Is<string>(s => exprectedReportOutput == s)));
        }

        private PowerTradeFilePublisher CreateUnitUnderTest()
        {
            return new PowerTradeFilePublisher(
                _mockLogger.Object,
                _mockConfiguration.Object,
                _mockFileSystemService.Object);
        }

        private IEnumerable<PowerTrade> CreatePowerTrades()
        {
            var random = new Random();
            var dateTime = DateTime.Now;
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