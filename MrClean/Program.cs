using MrClean.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddEnvironmentVariables();

        if (context.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets<Program>();
        }
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<DiscordBotService>();
    })
    .Build();

await host.RunAsync();