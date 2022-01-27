using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Extensions;
using MrClean.Models;

namespace MrClean.Commands.Filters;

public class CreateMessageFilterCommand : ISlashCommandProvider
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public CreateMessageFilterCommand(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder
        {
            Name = "create-filter",
            Description = "Creates a new message filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "pattern",
                    Description = "Regular expression pattern for matching the messages.",
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                },
                new()
                {
                    Name = "delay",
                    Description = "Delay in seconds between deleting / reposting the message.",
                    IsRequired = false,
                    Type = ApplicationCommandOptionType.Integer
                },
                new()
                {
                    Name = "repost-channel",
                    Description = "Channel, to which the deleted message should be reposted.",
                    IsRequired = false,
                    Type = ApplicationCommandOptionType.Channel
                }
            }
        }
        .Build();

    public async Task HandleCommandInvocationAsync(SocketSlashCommand command)
    {
        await command.DeferAsync();
        
        var pattern = command.GetOption<string>("pattern");
        var delay = (int) Math.Clamp(command.GetOption<long>("delay"), 0, 120);
        var channel = command.GetOption<SocketGuildChannel>("repost-channel");

        await using var context = await _factory.CreateDbContextAsync();

        var filter = new MessageFilter
        {
            Delay = delay,
            Pattern = pattern,
            RepostChannelId = channel?.Id,
        };

        await context.MessageFilters.AddAsync(filter);
        await context.SaveChangesAsync();

        await command.FollowupAsync(embed: filter.Embed);
    }
}