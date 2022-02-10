using Discord.Commands;
using MrClean.Models;

namespace MrClean.Commands.TypeReaders;

public class MessageFilterSpecificationTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        var output = input switch
        {
            "user" => TypeReaderResult.FromSuccess(MessageFilterSpecificationType.User),
            "role" => TypeReaderResult.FromSuccess(MessageFilterSpecificationType.Role),
            "channel" => TypeReaderResult.FromSuccess(MessageFilterSpecificationType.Channel),
            _ => TypeReaderResult.FromError(CommandError.Exception, "The parameter value must be either 'user', 'role' or 'channel'"),
        };

        return Task.FromResult(output);
    }
}