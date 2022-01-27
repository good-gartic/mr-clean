using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MrClean.Data;
using MrClean.Extensions;
using MrClean.Models;

namespace MrClean.Commands.Filters;

public class DenyMessageFilterSpecificationCommand : ISlashCommandProvider
{
    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public DenyMessageFilterSpecificationCommand(IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
    }

    public ApplicationCommandProperties Properties { get; } = new SlashCommandBuilder()
        {
            Name = "filter-deny",
            Description = "Add explicitly denied entities to the selected message filter",
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

        var user = command.GetOption<SocketGuildUser>("user");
        var role = command.GetOption<SocketRole>("role");
        var channel = command.GetOption<SocketGuildChannel>("channel");

        if (user != null) filter.UsersSpecification = filter.Users.AddDeniedEntity(user.Id).SpecificationString;
        if (role != null) filter.RolesSpecification = filter.Roles.AddDeniedEntity(role.Id).SpecificationString;
        if (channel != null) filter.ChannelsSpecification = filter.Channels.AddDeniedEntity(channel.Id).SpecificationString;


        context.MessageFilters.Update(filter);
            
        await context.SaveChangesAsync();
        await command.FollowupAsync(embed: filter.Embed);
    }
}