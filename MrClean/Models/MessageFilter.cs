using System.ComponentModel.DataAnnotations.Schema;
using Discord;

namespace MrClean.Models;

public class MessageFilter
{
    public int Id { get; set; }

    /// <summary>
    ///     Only apply this filter if it is enabled
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    ///     Delay in seconds, before the message is reposted / deleted
    /// </summary>
    public int Delay { get; set; } = 0;

    /// <summary>
    ///     A regular expression pattern, that determines, whether the filter should be applied
    /// </summary>
    public string? Pattern { get; set; } = null;

    /// <summary>
    ///     Id of the channel, to which the message should be reposted
    /// </summary>
    public ulong? RepostChannelId { get; set; } = null;

    /// <summary>
    ///     Filter specification restricting application of this filter to a selected channels only
    ///     If not present (or empty), this filter applies to all channels
    /// </summary>
    public string? ChannelsSpecification { get; set; } = null;

    [NotMapped]
    public MessageFilterSpecification Channels
    {
        get => new(ChannelsSpecification);
        set => ChannelsSpecification = value.SpecificationString;
    }
    /// <summary>
    ///     Filter specification restricting application of this filter to a selected users only
    ///     If not present (or empty), this filter applies to all users
    /// </summary>
    public string? UsersSpecification { get; set; } = null;

    [NotMapped]
    public MessageFilterSpecification Users
    {
        get => new(UsersSpecification);
        set => UsersSpecification = value.SpecificationString;
    }

    /// <summary>
    ///     Filter specification restricting application of this filter to a selected roles only
    ///     If not present (or empty), this filter applies to all roles
    /// </summary>
    public string? RolesSpecification { get; set; } = null;

    [NotMapped]
    public MessageFilterSpecification Roles
    {
        get => new(RolesSpecification);
        set => RolesSpecification = value.SpecificationString;
    }

    public Embed Embed
    {
        get
        {
            var emoji = Enabled ? "🟢" : "🔴";
            var delay = Delay <= 0 ? "_This filter is applied immediately_" : $"Applied after `{Delay}` seconds";
            var pattern = Pattern == null ? "_This filter will match all messages_" : $"`{Pattern}`";
            var reposting = RepostChannelId == null
                ? "_This filter does not repost matched messages_"
                : $"<#{RepostChannelId}>";

            var users = DescribeFilterSpecificationString(Users, e => $"<@{e}>");
            var roles = DescribeFilterSpecificationString(Roles, e => $"<@&{e}>");
            var channels = DescribeFilterSpecificationString(Channels, e => $"<#{e}>");

            return new EmbedBuilder()
                .WithColor(0x5865F2)
                .WithTitle($"{emoji} Message filter #{Id}")
                .WithFooter("This filter is now " + (Enabled ? "enabled" : "disabled"))
                .AddField("Delay before application", delay, true)
                .AddField("Regular expression", pattern, true)
                .AddField("Message reposting", reposting, true)
                .AddField("Users", users, true)
                .AddField("Roles", roles, true)
                .AddField("Channels", channels, true)
                .Build();
        }
    }

    private static string DescribeFilterSpecificationString(MessageFilterSpecification specification,
        Func<ulong, string> mapper)
    {
        var explicitlyAllowedEntities = specification.AllowedEntities.Count > 0;
        var explicitlyDeniedEntities = specification.DeniedEntities.Count > 0;

        return (explicitlyAllowedEntities, explicitlyDeniedEntities) switch
        {
            (false, false) => "Applies without any limitations",
            (false, true) => "Applies to everything except " +
                             string.Join(", ", specification.DeniedEntities.Select(mapper)),
            (true, _) => "Limited only to " + string.Join(", ", specification.AllowedEntities.Select(mapper))
        };
    }
}