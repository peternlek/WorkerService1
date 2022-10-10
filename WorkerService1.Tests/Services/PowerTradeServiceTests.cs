using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerService1.Interfaces;
using WorkerService1.Model;
using WorkerService1.Services;
using WorkerService1.Tests.Fakes;

namespace WorkerService1.Tests.Services
{
    [TestClass]
    public class PowerTradeServiceTests
    {
        private const string LongInterval = "1";
        private const string InvalidInterval = "-1";
        private const string ShortInterval = "0.05";
        private const string TestPublishDirectory = "TestDirectory";
        private const string InvalidPublishDirectory = "";

        private IList<string> _loggerMessages = new List<string>();

        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<PowerTradeService>> _mockLogger;
        private Mock<IPowerTradeRepository> _mockRespository;
        private Mock<IPowerTradePublisher> _mockPublisher;
        private Mock<IFileSystemService> _mockFileSystemService;
        private Mock<IConfigurationSection> _mockPublishDirectoryConfigurationSection;
        private Mock<IConfigurationSection> _mockPublishIntervalConfigurationSection;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<PowerTradeService>>();
            _mockRespository = new Mock<IPowerTradeRepository>();
            _mockPublisher = new Mock<IPowerTradePublisher>();
            _mockFileSystemService = new Mock<IFileSystemService>();
            _mockPublishDirectoryConfigurationSection = new Mock<IConfigurationSection>();
            _mockPublishIntervalConfigurationSection = new Mock<IConfigurationSection>();

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
        public void PowerTradeService_test_initialisation_successful_when_configuration_correct()
        {
            // Arrange
            var taskCancelled = false;
            _mockPublishDirectoryConfigurationSection
                .Setup(x => x.Value)
                .Returns(TestPublishDirectory);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}"))
                .Returns(_mockPublishDirectoryConfigurationSection.Object).Verifiable();

            _mockPublishIntervalConfigurationSection
                .Setup(x => x.Value)
                .Returns(LongInterval);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingIntervalMinutes}"))
                .Returns(_mockPublishIntervalConfigurationSection.Object).Verifiable();

            var uut = CreateUnitUnderTest();

            // Act
            Task.Run(async () =>
            {
                try
                {
                    await uut.FakeExecuteAsync(300);
                }
                catch (OperationCanceledException e) {}
            }).Wait();

            // Assert
            _mockConfiguration.VerifyAll();
            Assert.IsFalse(_loggerMessages.Any(m => m == Resources.Resources.LogMessageInvalidPublishDirectory));
            Assert.IsFalse(_loggerMessages.Any(m => m == Resources.Resources.LogMessageInvalidPublishInterval));
        }

        [TestMethod]
        public void PowerTradeService_test_initialisation_unsuccessful_when_configuration_incorrect()
        {
            // Arrange
            _mockPublishDirectoryConfigurationSection
                .Setup(x => x.Value)
                .Returns(InvalidPublishDirectory);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}"))
                .Returns(_mockPublishDirectoryConfigurationSection.Object).Verifiable();

            _mockPublishIntervalConfigurationSection
                .Setup(x => x.Value)
                .Returns(InvalidInterval);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingIntervalMinutes}"))
                .Returns(_mockPublishIntervalConfigurationSection.Object).Verifiable();

            var uut = CreateUnitUnderTest();

            // Act
            Task.Run(async () =>
            {
                try
                {
                    await uut.FakeExecuteAsync(300);
                }
                catch (OperationCanceledException e) {}
            }).Wait();

            // Assert
            _mockConfiguration.VerifyAll();
            Assert.IsTrue(_loggerMessages.Any(m => m == Resources.Resources.LogMessageInvalidPublishDirectory));
            Assert.IsTrue(_loggerMessages.Any(m => m == Resources.Resources.LogMessageInvalidPublishInterval));
        }

        [TestMethod]
        public void PowerTradeService_verify_calls_to_retrieve_and_publish_aggregated_power_trades()
        {
            // Arrange
            var aggreatedPowerTrades = new AggregatedPowerTrades();
            _mockPublishDirectoryConfigurationSection
                .Setup(x => x.Value)
                .Returns(TestPublishDirectory);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingDirectory}"))
                .Returns(_mockPublishDirectoryConfigurationSection.Object).Verifiable();

            _mockPublishIntervalConfigurationSection
                .Setup(x => x.Value)
                .Returns(ShortInterval);

            _mockConfiguration
                .Setup(x => x.GetSection($"{Resources.Resources.ConfigKeyAppSettings}:{Resources.Resources.ConfigKeyPowerTradePublishingIntervalMinutes}"))
                .Returns(_mockPublishIntervalConfigurationSection.Object).Verifiable();

            _mockRespository.Setup(r => r.GetAggregatedPowerTrades())
                .Returns(Task.FromResult(aggreatedPowerTrades));

            var uut = CreateUnitUnderTest();

            // Act
            Task.Run(async () =>
            {
                try
                {
                    await uut.FakeExecuteAsync(4000);
                }
                catch (OperationCanceledException e) { }
            }).Wait();

            // Assert
            _mockRespository.Verify(r => r.GetAggregatedPowerTrades(), Times.Exactly(2));
            _mockPublisher.Verify(r => r.Publish(aggreatedPowerTrades), Times.Exactly(2));
        }

        private TestPowerTradeService CreateUnitUnderTest()
        {
            return new TestPowerTradeService(
                _mockLogger.Object,
                _mockConfiguration.Object,
                _mockPublisher.Object,
                _mockRespository.Object,
                _mockFileSystemService.Object);
        }
    }
}