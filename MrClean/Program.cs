using MrClean.Configuration;
using MrClean.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<DiscordBotService>();
        services.Configure<DiscordOptions>(context.Configuration.GetRequiredSection(DiscordOptions.Section));
    })
    .Build();

await host.RunAsync();