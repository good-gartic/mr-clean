using Microsoft.EntityFrameworkCore;
using MrClean.Commands;
using MrClean.Commands.Filters;
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

        services.AddTransient<ISlashCommandProvider, ListMessageFiltersCommand>();
        services.AddTransient<ISlashCommandProvider, CreateMessageFilterCommand>();
        services.AddTransient<ISlashCommandProvider, DeleteMessageFilterCommand>();
        services.AddTransient<ISlashCommandProvider, AllowMessageFilterSpecificationCommand>();
        services.AddTransient<ISlashCommandProvider, DenyMessageFilterSpecificationCommand>();
        
        services.AddTransient<SlashCommandDispatcher>();
        services.AddHostedService<DiscordBotService>();
        services.AddDbContextFactory<MrCleanDbContext>(options =>
        {
            options.UseNpgsql(context.Configuration.GetConnectionString("Default"));
        });
    })
    .Build();

await host.RunAsync();