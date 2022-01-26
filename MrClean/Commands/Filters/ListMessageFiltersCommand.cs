using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;

namespace MrClean.Commands.Filters;

public class ListMessageFiltersCommand : ISlashCommandProvider
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public ListMessageFiltersCommand(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder
        {
            Name = "list-filters",
            Description = "List all enabled message filters",
        }
        .Build();

    public async Task HandleCommandInvocationAsync(SocketSlashCommand command)
    {
        await command.DeferAsync();

        await using var context = await _factory.CreateDbContextAsync();

        var filters = await context.MessageFilters.ToListAsync();

        if (filters.Count == 0)
        {
            await command.FollowupAsync(embeds: filters.Select(f => f.Embed).ToArray());
            return;
        }

        await command.FollowupAsync(embed: new EmbedBuilder()
            .WithColor(0xFEE75C)
            .WithTitle("Sorry, there are no enabled message filters")
            .WithDescription("To create a new message filter, use the `/create-filter` command")
            .Build()
        );
    }
}