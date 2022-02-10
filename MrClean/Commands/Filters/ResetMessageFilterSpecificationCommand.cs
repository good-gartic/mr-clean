using Discord;
using Discord.WebSocket;
using MrClean.Exceptions;
using MrClean.Extensions;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class ResetMessageFilterSpecificationCommand : ISlashCommandProvider
{
    private readonly IMessageFiltersService _service;

    public ResetMessageFilterSpecificationCommand(IMessageFiltersService service)
    {
        _service = service;
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

        try
        {
            var id = (int) command.GetOption<long>("id");
            var filter = await _service.ResetFilterSpecificationAsync(id);

            await command.FollowupAsync(embed: filter.Embed);
        }
        catch (MessageFilterNotFoundException)
        {
            await command.FollowupAsync(embed: new EmbedBuilder()
                .WithColor(0xED4245)
                .WithTitle("Sorry, message filter with matching ID not found.")
                .WithDescription("To list all available filters, use the `/list-filters` command.")
                .Build()
            );
        }
    }
}