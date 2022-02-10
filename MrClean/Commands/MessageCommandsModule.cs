using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Query;
using MrClean.Services.Filters;

namespace MrClean.Commands;

public class MessageCommandsModule : ModuleBase<SocketCommandContext>
{
    private readonly IMessageFiltersService _service;

    public MessageCommandsModule(IMessageFiltersService service)
    {
        _service = service;
    }

    [Command("create")]
    public async Task CreateMessageFilterCommand(string pattern, int? delay = null, SocketTextChannel? repostChannel = null)
    {
        var filter = await _service.CreateMessageFilterAsync(pattern, delay ?? 0, repostChannel?.Id);

        await Context.Message.ReplyAsync(embed: filter.Embed);
    }

    [Command("delete")]
    public async Task DeleteMessageFilterCommand(int id)
    {
        await _service.DeleteMessageFilterAsync(id);
        await Context.Message.ReplyAsync(embed: new EmbedBuilder()
            .WithColor(0x5865F2)
            .WithTitle($"Message filter #{id} deleted")
            .Build()
        );
    }

    [Command("enable")]
    public async Task EnableFilterCommand(int id)
    {
        var filter = await _service.EnableMessageFilterAsync(id);
        await Context.Message.ReplyAsync(embed: filter.Embed);
    }

    [Command("disable")]
    public async Task DisableFilterCommand(int id)
    {
        var filter = await _service.DisableMessageFilterAsync(id);
        await Context.Message.ReplyAsync(embed: filter.Embed);
    }

    [Command("list")]
    public async Task ListFiltersCommand()
    {
        var filters = await _service.ListMessageFiltersAsync();
        var embeds = filters.Select(f => f.Embed).ToArray();

        await Context.Message.ReplyAsync(embeds: embeds);
    }
}