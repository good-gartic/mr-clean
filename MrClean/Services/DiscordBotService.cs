using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrClean.Commands;
using MrClean.Configuration;
using MrClean.Data;

namespace MrClean.Services;

public class DiscordBotService : BackgroundService
{
    private readonly ILogger<DiscordBotService> _logger;

    private readonly DiscordOptions _options;

    private readonly DiscordSocketClient _client;

    private readonly SlashCommandDispatcher _dispatcher;

    private readonly MessageFilteringService _filter;

    private readonly IDbContextFactory<MrCleanDbContext> _contextFactory;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        IOptions<DiscordOptions> options,
        IDbContextFactory<MrCleanDbContext> contextFactory,
        SlashCommandDispatcher dispatcher,
        MessageFilteringService filter
    )
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _options = options.Value;
        _dispatcher = dispatcher;
        _filter = filter;

        var config = new DiscordSocketConfig()
        {
            AlwaysDownloadUsers = true,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        };

        _client = new DiscordSocketClient(config);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Preparing the database");

        await UpdateDatabaseAsync();

        _logger.LogInformation("Starting the Mr.Clean Discord bot at {time}", DateTimeOffset.Now);

        _client.Ready += HandleReadyEventAsync;
        _client.Connected += HandleConnectedEventAsync;
        _client.Log += HandleLogMessageAsync;

        _filter.RegisterMessageHandler(_client);

        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();
        await Task.Delay(-1, stoppingToken);
    }

    private async Task UpdateDatabaseAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    private async Task HandleReadyEventAsync()
    {
        try
        {
            var guild = _client.GetGuild(_options.GuildId);

            // Validate, that the guild is configuration is correct
            if (guild == null)
            {
                throw new ApplicationException($"The configured guild (id = {_options.GuildId}) cannot be found!");
            }

            await _client.SetGameAsync(type: ActivityType.Watching, name: $"for messages in {guild.Name}");
            await _dispatcher.RegisterSlashCommandsAsync(_client);
        }
        catch (Exception exception)
        {
            _logger.LogCritical("Exception in ready event handler: {exception}", exception);
            throw;
        }
    }

    private Task HandleConnectedEventAsync()
    {
        _logger.LogInformation("Connected to the Discord gateway");
        return Task.CompletedTask;
    }

    private Task HandleLogMessageAsync(LogMessage message)
    {
        _logger.LogInformation(message.Message);
        return Task.CompletedTask;
    }
}