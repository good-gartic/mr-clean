using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Extensions;

namespace MrClean.Commands.Filters;

public class ResetMessageFilterSpecificationCommand : ISlashCommandProvider
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public ResetMessageFilterSpecificationCommand(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder
        {
            Name = "filter-reset",
            Description = "Reset all message filters specifications for the selected message filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "id",
                    Description = "ID of the filter that should be reset",
                    Type = ApplicationCommandOptionType.Integer,
                    IsRequired = true
                }
            }
        }
        .Build();

    public async Task HandleCommandInvocationAsync(SocketSlashCommand command)
    {
        await command.DeferAsync();

        await using var context = await _factory.CreateDbContextAsync();

        var id = command.GetOption<long>("id");
        var filter = await context.MessageFilters.FirstOrDefaultAsync(f => f.Id == id);

        if (filter == null)
        {
            await command.FollowupAsync(embed: new EmbedBuilder()
                .WithColor(0xED4245)
                .WithTitle("Sorry, message filter with matching ID not found.")
                .WithDescription("To list all available filters, use the `/list-filters` command.")
                .Build()
            );
            return;
        }

        filter.UsersSpecification = null;
        filter.RolesSpecification = null;
        filter.ChannelsSpecification = null;

        context.MessageFilters.Update(filter);

        await context.SaveChangesAsync();
        await command.FollowupAsync(embed: filter.Embed);
    }
}