using System.Runtime.InteropServices;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Exceptions;
using MrClean.Extensions;
using MrClean.Models;
using MrClean.Services.Filters;

namespace MrClean.Commands.Filters;

public class AllowMessageFilterSpecificationCommand : ISlashCommandProvider
{
    private readonly MessageFiltersService _service;

    public AllowMessageFilterSpecificationCommand(MessageFiltersService service)
    {
        _service = service;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder()
        {
            Name = "filter-allow",
            Description = "Add explicitly allowed entities to the selected message filter",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "id",
                    Description = "ID of the filter that should be modified",
                    Type = ApplicationCommandOptionType.Integer,
                    IsRequired = true
                },
                new()
                {
                    Name = "user",
                    Description = "Add the mentioned user to message filter specification",
                    Type = ApplicationCommandOptionType.User
                },
                new()
                {
                    Name = "role",
                    Description = "Add the mentioned role to message filter specification",
                    Type = ApplicationCommandOptionType.Role
                },
                new()
                {
                    Name = "channel",
                    Description = "Add the mentioned channel to message filter specification",
                    Type = ApplicationCommandOptionType.Channel
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
            var filter = await _service.GetMessageFilterAsync(id);
            
            var user = command.GetOption<SocketGuildUser>("user");
            var role = command.GetOption<SocketRole>("role");
            var channel = command.GetOption<SocketGuildChannel>("channel");

            if (user != null) await _service.AddAllowedEntityAsync(id, MessageFilterSpecificationType.User, user.Id);
            if (role != null) await _service.AddAllowedEntityAsync(id, MessageFilterSpecificationType.User, role.Id);
            if (channel != null) await _service.AddAllowedEntityAsync(id, MessageFilterSpecificationType.User, channel.Id);

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