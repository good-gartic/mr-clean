using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MrClean.Configuration;

namespace MrClean.Services;

public class DiscordBotService : BackgroundService
{
    private readonly ILogger<DiscordBotService> _logger;

    private readonly DiscordOptions _options;

    private readonly DiscordSocketClient _client;

    public DiscordBotService(ILogger<DiscordBotService> logger, IOptions<DiscordOptions> options)
    {
        _logger = logger;
        _options = options.Value; 
        _client = new DiscordSocketClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the Mr.Clean Discord bot at {time}", DateTimeOffset.Now);

        _client.Ready += HandleReadyEvent;

        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();
        await Task.Delay(-1, stoppingToken);
    }

    private async Task HandleReadyEvent()
    {
        await _client.SetGameAsync(type: ActivityType.Watching, name: "for messages");
    }
}