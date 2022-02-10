using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Extensions;
using MrClean.Models;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class CreateMessageFilterCommand : ISlashCommandProvider
{
    private readonly IMessageFiltersService _service;

    public CreateMessageFilterCommand(IMessageFiltersService service)
    {
        _service = service;
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
        
        var pattern = command.GetOption<string>("pattern") ?? throw new ArgumentNullException(nameof(command));
        var delay = (int) Math.Clamp(command.GetOption<long>("delay"), 0, 120);
        var channel = command.GetOption<SocketGuildChannel>("repost-channel");

        var filter = await _service.CreateMessageFilterAsync(pattern, delay, channel?.Id);

        await command.FollowupAsync(embed: filter.Embed);
    }
}