using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MrClean.Configuration;

namespace MrClean.Commands;

public class SlashCommandDispatcher
{
    private readonly DiscordOptions _options;

    private readonly ILogger<SlashCommandDispatcher> _logger;

    private readonly IEnumerable<ISlashCommandProvider> _commands;

    public SlashCommandDispatcher(
        IOptions<DiscordOptions> options,
        ILogger<SlashCommandDispatcher> logger,
        IEnumerable<ISlashCommandProvider> commands
    )
    {
        _options = options.Value;
        _logger = logger;
        _commands = commands;
    }

    public async Task RegisterSlashCommandsAsync(DiscordSocketClient client)
    {
        var guild = client.GetGuild(_options.GuildId);
        var commands = _commands.Select(c => c.Properties).ToArray();

        await guild.BulkOverwriteApplicationCommandAsync(commands);
    }

    private async Task DispatchCommandAsync(SocketSlashCommand command)
    {
        var handler = _commands.FirstOrDefault(c =>
            c.Properties.Name.IsSpecified &&
            c.Properties.Name.Value == command.CommandName
        );

        if (handler == null)
        {
            _logger.LogWarning("Missing slash command handler for /{command}", command.CommandName);
            return;
        }

        await handler.HandleCommandInvocationAsync(command);
    }
}