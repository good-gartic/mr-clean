using Discord;
using Discord.WebSocket;

namespace MrClean.Commands.Filters;

public class CreateMessageFilterCommand : ISlashCommandProvider
{
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
                    IsRequired = false,
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
        await command.RespondAsync("Working on it...");
    }
}