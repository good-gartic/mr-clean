using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class ListMessageFiltersCommand : ISlashCommandProvider
{
    private readonly IMessageFiltersService _service;

    public ListMessageFiltersCommand(IMessageFiltersService service)
    {
        _service = service;
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

        var filters = await _service.ListMessageFiltersAsync();

        if (filters.Count > 0)
        {
            await command.FollowupAsync(embeds: filters.Select(f => f.Embed).ToArray());
            return;
        }

        await command.FollowupAsync(embed: new EmbedBuilder()
            .WithColor(0xFEE75C)
            .WithTitle("Sorry, there are no message filters")
            .WithDescription("To create a new message filter, use the `/create-filter` command")
            .Build()
        );
    }
}