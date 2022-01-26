using Discord.WebSocket;

namespace MrClean.Extensions;

public static class SocketSlashCommandExtensions
{
    public static T? GetOption<T>(this SocketSlashCommand command, string name)
    {
        var option = command.Data.Options.FirstOrDefault(o => o.Name == name);
        var value = option?.Value;
        
        return value is T casted ? casted : default;
    }
}