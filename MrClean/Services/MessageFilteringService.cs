using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrClean.Configuration;
using MrClean.Data;
using MrClean.Models;

namespace MrClean.Services;

public class MessageFilteringService
{
    private readonly DiscordOptions _options;

    private readonly IDbContextFactory<MrCleanDbContext> _factory;

    public MessageFilteringService(IOptions<DiscordOptions> options, IDbContextFactory<MrCleanDbContext> factory)
    {
        _factory = factory;
        _options = options.Value;
    }

    public void RegisterMessageHandler(DiscordSocketClient client)
    {
        client.MessageReceived += async message => await HandleMessageEventAsync(message, client);
    }

    private async Task HandleMessageEventAsync(SocketMessage message, BaseSocketClient client)
    {
        // If the message was not sent in the managed guild
        if (
            message.Channel is not SocketGuildChannel channel ||
            message.Author.Id == client.CurrentUser.Id ||
            channel.Guild.Id != _options.GuildId
        )
        {
            return;
        }

        await using var context = await _factory.CreateDbContextAsync();

        var filters = await context.MessageFilters.ToListAsync();
        var matching = filters.FirstOrDefault(f => Matches(f, message));

        if (matching == null)
        {
            return;
        }
        
        await Task.Delay(matching.Delay * 1000);

        if (matching.RepostChannelId is not null)
        {
            var repostChannel = channel.Guild.GetTextChannel(matching.RepostChannelId.Value);

            await repostChannel.SendMessageAsync(
                text: message.Content,
                embeds: message.Embeds.ToArray(),
                stickers: message.Stickers.ToArray(),
                messageReference: message.Reference 
            );
        }

        await message.DeleteAsync();
    }

    private static bool Matches(MessageFilter filter, SocketMessage message)
    {
        var pattern = new Regex(filter.Pattern ?? ".*");

        if (!pattern.IsMatch(message.CleanContent))
        {
            return false;
        }

        return filter.Channels.AllowsEntity(message.Channel.Id) && filter.Users.AllowsEntity(message.Author.Id);
    }
}