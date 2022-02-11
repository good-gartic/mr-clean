using Discord;
using Discord.WebSocket;
using MrClean.Extensions;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class EditMessageFilterCommand : ISlashCommandProvider
{
    private readonly IMessageFiltersService _service;

    public EditMessageFilterCommand(IMessageFiltersService service)
    {
        _service = service;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder
        {
            Name = "edit-filter",
            Description = "Edits values of the selected filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "id",
                    Description = "ID of the filter that should be edited",
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.Integer
                },
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

        var id = (int) command.GetOption<long>("id");
        var pattern = command.GetOption<string>("pattern") ?? throw new ArgumentNullException(nameof(command));
        var delay = (int) Math.Clamp(command.GetOption<long>("delay"), 0, 120);
        var channel = command.GetOption<SocketGuildChannel>("repost-channel");

        var filter = await _service.EditMessageFilterAsync(id, pattern, delay, channel?.Id);

        await command.FollowupAsync(embed: filter.Embed);
    }
}