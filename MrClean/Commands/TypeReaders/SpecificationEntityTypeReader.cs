using Discord.Commands;
using MrClean.Models;

namespace MrClean.Commands.TypeReaders;

public class SpecificationEntityTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        var output = input switch
        {
            "user" => TypeReaderResult.FromSuccess(SpecificationEntityType.User),
            "role" => TypeReaderResult.FromSuccess(SpecificationEntityType.Role),
            "channel" => TypeReaderResult.FromSuccess(SpecificationEntityType.Channel),
            _ => TypeReaderResult.FromError(CommandError.Exception,
                "The parameter value must be either 'user', 'role' or 'channel'"),
        };

        return Task.FromResult(output);
    }
}