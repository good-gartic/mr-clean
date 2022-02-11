using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;

namespace MrClean.Commands.TypeReaders;

public class SnowflakeEntityTypeReader : TypeReader
{
    private record FakeSnowflakeEntity(ulong Id) : ISnowflakeEntity
    {
        public ulong Id { get; } = Id;

        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
    }
    
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        // This is just a quick hack to make mentioning users / roles / channels working
        var pattern = new Regex(@"<[#@][&!]?(\d+)>");
        var match = ulong.Parse(pattern.Match(input).Groups[1].Value);
        var result = TypeReaderResult.FromSuccess(new FakeSnowflakeEntity(match));
            
        return Task.FromResult(result);
    }
}