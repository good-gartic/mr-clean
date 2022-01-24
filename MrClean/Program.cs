using MrClean;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DiscordBot>();
    })
    .Build();

await host.RunAsync();