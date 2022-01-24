namespace MrClean;

public class DiscordBot : BackgroundService
{
    private readonly ILogger<DiscordBot> _logger;

    public DiscordBot(ILogger<DiscordBot> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting the Mr.Clean Discord bot at {time}", DateTimeOffset.Now);
        
        await Task.Delay(-1, stoppingToken);
    }
}