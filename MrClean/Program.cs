using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MrClean.Configuration;
using MrClean.Data;
using MrClean.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<DiscordOptions>(context.Configuration.GetRequiredSection(DiscordOptions.Section));
        
        services.AddHostedService<DiscordBotService>();
        services.AddDbContextFactory<MrCleanDbContext>(options =>
        {
            options.UseNpgsql(context.Configuration.GetConnectionString("Default"));
        });
    })
    .Build();

await host.RunAsync();