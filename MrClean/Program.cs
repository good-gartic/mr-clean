using Microsoft.EntityFrameworkCore;
using MrClean.Commands;
using MrClean.Commands.Filters;
using MrClean.Configuration;
using MrClean.Data;
using MrClean.Extensions;
using MrClean.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
    .ConfigureServices((context, services) =>
    {
        services.Configure<DiscordOptions>(context.Configuration.GetRequiredSection(DiscordOptions.Section));

        services.AddCommands(collection => collection
            .AddCommand<ListMessageFiltersCommand>()
            .AddCommand<CreateMessageFilterCommand>()
            .AddCommand<DeleteMessageFilterCommand>()
            .AddCommand<AllowMessageFilterSpecificationCommand>()
            .AddCommand<DenyMessageFilterSpecificationCommand>()
            .AddCommand<ResetMessageFilterSpecificationCommand>()
            .AddCommand<EnableFilterCommand>()
            .AddCommand<DisableFilterCommand>()
        );

        services.AddTransient<MessageFilteringService>();
        services.AddHostedService<DiscordBotService>();
        services.AddDbContextFactory<MrCleanDbContext>(options =>
        {
            options.UseNpgsql(context.Configuration.GetConnectionString("Default"));
        });
    })
    .Build();

await host.RunAsync();