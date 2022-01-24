using MrClean;
using MrClean.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DiscordBotService>();
    })
    .Build();

await host.RunAsync();