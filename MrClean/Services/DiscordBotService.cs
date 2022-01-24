namespace MrClean.Services;

public class DiscordBotService : BackgroundService
{
    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(ILogger<DiscordBotService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the Mr.Clean Discord bot at {time}", DateTimeOffset.Now);
        
        await Task.Delay(-1, stoppingToken);
    }
}