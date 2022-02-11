using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using MrClean.Commands;
using MrClean.Commands.Filters;
using MrClean.Configuration;
using MrClean.Data;
using MrClean.Extensions;
using MrClean.Services;
using MrClean.Services.Filters;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
    .ConfigureServices((context, services) =>
    {
        services.Configure<DiscordOptions>(context.Configuration.GetRequiredSection(DiscordOptions.Section));

        services.AddCommands(collection => collection
            .AddCommand<AllowMessageFilterSpecificationCommand>()
            .AddCommand<CreateMessageFilterCommand>()
            .AddCommand<EditMessageFilterCommand>()
            .AddCommand<DeleteMessageFilterCommand>()
            .AddCommand<DenyMessageFilterSpecificationCommand>()
            .AddCommand<DisableFilterCommand>()
            .AddCommand<EnableFilterCommand>()
            .AddCommand<ListMessageFiltersCommand>()
            .AddCommand<ResetMessageFilterSpecificationCommand>()
        );

        services.AddTransient<MessageFilteringService>();
        services.AddSingleton<IMessageFiltersService, MessageFiltersService>();
        services.AddSingleton<CommandService>();
        services.AddHostedService<DiscordBotService>();
        services.AddDbContextFactory<MrCleanDbContext>(options =>
        {
            options.UseNpgsql(context.Configuration.GetConnectionString("Default"));
        });
    })
    .Build();

await host.RunAsync();