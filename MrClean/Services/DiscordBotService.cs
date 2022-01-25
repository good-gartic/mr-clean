using Microsoft.Extensions.Options;
using MrClean.Configuration;

namespace MrClean.Services;

public class DiscordBotService : BackgroundService
{
    private readonly ILogger<DiscordBotService> _logger;

    private readonly DiscordOptions _options;

    public DiscordBotService(ILogger<DiscordBotService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _options = configuration.GetRequiredSection(DiscordOptions.Section).Get<DiscordOptions>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the Mr.Clean Discord bot at {time}", DateTimeOffset.Now);
        _logger.LogInformation("Starting with token {token}", _options.Token);
        
        await Task.Delay(-1, stoppingToken);
    }
}