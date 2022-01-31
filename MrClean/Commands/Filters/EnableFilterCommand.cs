using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Extensions;

namespace MrClean.Commands.Filters;

public class EnableFilterCommand : ISlashCommandProvider
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public EnableFilterCommand(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder()
        {
            Name = "enable-filter",
            Description = "Enables the specified message filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "id",
                    Description = "ID of the filter that should be enabled",
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

        filter.Enabled = true;
        context.MessageFilters.Update(filter);
        
        await context.SaveChangesAsync();
        await command.FollowupAsync(embed: new EmbedBuilder()
            .WithColor(0x5865F2)
            .WithTitle($"Message filter #{filter.Id} enabled")
            .Build()
        );
    }
}