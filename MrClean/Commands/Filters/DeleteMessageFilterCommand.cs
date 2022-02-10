using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Exceptions;
using MrClean.Extensions;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class DeleteMessageFilterCommand : ISlashCommandProvider
{
    private readonly IMessageFiltersService _service;

    public DeleteMessageFilterCommand(IMessageFiltersService service)
    {
        _service = service;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder()
        {
            Name = "delete-filter",
            Description = "Deletes the specified message filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "id",
                    Description = "ID of the filter that should be deleted",
                    Type = ApplicationCommandOptionType.Integer,
                    IsRequired = true
                }
            }
        }
        .Build();

    public async Task HandleCommandInvocationAsync(SocketSlashCommand command)
    {
        await command.DeferAsync();

        var id = (int) command.GetOption<long>("id");

        try
        {
            await _service.DeleteMessageFilterAsync(id);
            await command.FollowupAsync(embed: new EmbedBuilder()
                .WithColor(0x5865F2)
                .WithTitle($"Message filter #{id} deleted")
                .Build()
            );
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