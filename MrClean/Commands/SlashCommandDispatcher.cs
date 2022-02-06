using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using MrClean.Configuration;

namespace MrClean.Commands;

public class SlashCommandDispatcher
{
    private readonly DiscordOptions _options;

    private readonly ILogger<SlashCommandDispatcher> _logger;

    private readonly IEnumerable<ISlashCommandProvider> _commands;

    private readonly IHostEnvironment _environment;

    public SlashCommandDispatcher(
        IOptions<DiscordOptions> options,
        ILogger<SlashCommandDispatcher> logger,
        IEnumerable<ISlashCommandProvider> commands,
        IHostEnvironment environment
    )
    {
        _options = options.Value;
        _logger = logger;
        _commands = commands;
        _environment = environment;
    }

    public async Task RegisterSlashCommandsAsync(DiscordSocketClient client)
    {
        client.SlashCommandExecuted += DispatchCommandAsync;

        var guild = client.GetGuild(_options.GuildId);
        var commands = _commands.Select(c => c.Properties).ToArray();

        // If running in the production mode, register the commands globally instead of using guild-local ones
        if (_environment.IsDevelopment())
        {
            await guild.BulkOverwriteApplicationCommandAsync(commands);
            await guild.DownloadUsersAsync();
            return;
        }
        
        await client.BulkOverwriteGlobalApplicationCommandsAsync(commands);
    }

    private async Task DispatchCommandAsync(SocketSlashCommand command)
    {
        var handler = _commands.FirstOrDefault(c =>
            c.Properties.Name.IsSpecified &&
            c.Properties.Name.Value == command.CommandName
        );

        if (handler == null)
        {
            _logger.LogWarning("Missing slash command handler for /{command}", command.CommandName);

            await command.RespondAsync(
                ephemeral: true,
                components: new ComponentBuilder()
                    .WithButton(
                        ButtonBuilder.CreateLinkButton(
                            "🪲 Open a new issue",
                            "https://github.com/good-gartic/mr-clean/issues/new"
                        )
                    )
                    .Build(),
                embed: new EmbedBuilder()
                    .WithColor(0xED4245)
                    .WithTitle("I'm sorry, but this command was not found")
                    .WithDescription("This is probably a bug within my code.")
                    .Build()
            );

            return;
        }

        // It's safe to typecast here, as the command is registered guild-only
        var guild = (command.Channel as SocketGuildChannel)!.Guild;
        var member = guild.GetUser(command.User.Id);

        // If the member doesn't have the MANAGE_GUILD permissions
        if (member is not {GuildPermissions.ManageGuild: true})
            await command.RespondAsync(
                ephemeral: true,
                components: new ComponentBuilder()
                    .WithButton(
                        ButtonBuilder.CreateLinkButton(
                            "🪲 Open a new issue",
                            "https://github.com/good-gartic/mr-clean/issues/new"
                        )
                    )
                    .Build(),
                embed: new EmbedBuilder()
                    .WithColor(0xED4245)
                    .WithTitle("I'm sorry, but this command can be used only by the mods.")
                    .WithDescription(
                        "If you think you should be allowed to invoke this command, please open a new issue.")
                    .Build()
            );

        await handler.HandleCommandInvocationAsync(command);
    }
}