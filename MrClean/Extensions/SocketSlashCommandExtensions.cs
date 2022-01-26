using Discord.WebSocket;

namespace MrClean.Extensions;

public static class SocketSlashCommandExtensions
{
    public static T? GetOption<T>(this SocketSlashCommand command, string name, T? defaultValue = default)
    {
        var value = command.Data.Options.FirstOrDefault(o => o.Name == name)?.Value;
        
        return value is T type ? type : defaultValue;
    }
}