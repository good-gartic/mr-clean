using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MrClean.Configuration;

namespace MrClean.Commands;

public class SlashCommandDispatcher
{
    private readonly DiscordOptions _options;

    private readonly IEnumerable<ISlashCommandProvider> _commands;

    public SlashCommandDispatcher(IOptions<DiscordOptions> options, IEnumerable<ISlashCommandProvider> commands)
    {
        _commands = commands;
        _options = options.Value;
    }

    public async Task RegisterSlashCommandsAsync(DiscordSocketClient client)
    {
        var guild = client.GetGuild(_options.GuildId);
        var commands = _commands.Select(c => c.Properties).ToArray();

        await guild.BulkOverwriteApplicationCommandAsync(commands);
    }

    private async Task DispatchCommandAsync(SocketSlashCommand command)
    {
    }
}