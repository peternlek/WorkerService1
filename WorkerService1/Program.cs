using NLog.Web;
using Services;
using WorkerService1.Interfaces;
using WorkerService1.Publishers;
using WorkerService1.Repositories;
using WorkerService1.Resources;
using WorkerService1.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = Resources.ServiceName;
    })
    .ConfigureServices(services =>
    {
        services.AddLogging(logger =>
        {
            logger.ClearProviders();
        });

        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<IPowerService, PowerService>();
        services.AddSingleton<IPowerTradePublisher, PowerTradeFilePublisher>();
        services.AddSingleton<IPowerTradeRepository, PowerTradeRepository>();
        services.AddHostedService<PowerTradeService>();
    })
    .UseNLog()
    .Build();

await host.RunAsync();
