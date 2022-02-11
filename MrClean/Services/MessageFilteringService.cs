using System.Text.RegularExpressions;
using Discord;
using Discord.Webhook;
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
            message.Author.IsWebhook ||
            message.Channel is not SocketGuildChannel channel ||
            message.Author.Id == client.CurrentUser.Id ||
            channel.Guild.Id != _options.GuildId
        )
        {
            return;
        }

        await using var context = await _factory.CreateDbContextAsync();

        var filters = await context.MessageFilters.Where(f => f.Enabled).ToListAsync();
        var matching = filters.FirstOrDefault(f => Matches(f, message));

        if (matching == null)
        {
            return;
        }

        await Task.Delay(matching.Delay * 1000);

        if (matching.RepostChannelId is not null)
        {
            var repostChannel = channel.Guild.GetTextChannel(matching.RepostChannelId.Value);
            
            // If there are no webhooks in this channel, create a new one
            var webhooks = await repostChannel.GetWebhooksAsync();
            var webhook = webhooks.FirstOrDefault() ?? await repostChannel.CreateWebhookAsync("mr-clean-webhook");

            var webhookClient = new DiscordWebhookClient(webhook);
            var repostedMessageId = await webhookClient.SendMessageAsync(
                message.Content,
                username: message.Author.Username,
                avatarUrl: message.Author.GetAvatarUrl(),
                embeds: message.Embeds.ToArray()
            );

            if (message.Attachments.Count != 0)
            {
                // Repost all message attachments (files, images, videos...)
                var httpClient = new HttpClient();
                var attachments = await Task.WhenAll(
                    message.Attachments.Select(async a =>
                        new FileAttachment( await httpClient.GetStreamAsync(a.Url),
                            a.Filename,
                            a.Filename,
                            a.IsSpoiler()
                        )
                    )
                );

                var repostedMessage = await repostChannel.GetMessageAsync(repostedMessageId);

                await webhookClient.SendFilesAsync(
                    text: string.Empty,
                    attachments: attachments,
                    username: message.Author.Username,
                    avatarUrl: message.Author.GetAvatarUrl()
                    
                );
            }
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