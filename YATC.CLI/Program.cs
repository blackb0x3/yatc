using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using YATC.CLI;
using YATC.CLI.IoC;
using YATC.Core;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

// create hosting object and DI layer
using IHost host = CreateHostBuilder().Build();

// create a service scope
using var scope = host.Services.CreateScope();

var serviceProvider = scope.ServiceProvider;

try
{
    var cts = new CancellationTokenSource();

    Console.CancelKeyPress += async (_, e) =>
    {
        e.Cancel = true;
        await cts.CancelAsync();
    };

    var app = serviceProvider.GetRequiredService<App>();
    await app.Run(args, cts.Token);
}
catch (Exception e)
{
    var logger = serviceProvider.GetService<IYatcLogger>();
    logger.LogError(new { exception = e.Message, e.StackTrace });
}


IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            Installer.Install(services);
            services.AddSingleton<App>();
        });
}
