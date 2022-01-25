using Discord;
using Discord.WebSocket;

namespace MrClean.Commands;

public interface ISlashCommandProvider
{
    public ApplicationCommandProperties Properties { get; }

    public Task HandleCommandInvocationAsync(SocketSlashCommand command);
}