using Discord;

namespace MrClean.Models;

public class MessageFilter
{
    public int Id { get; set; }

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

    /// <summary>
    ///     Filter specification restricting application of this filter to a selected users only
    ///     If not present (or empty), this filter applies to all users
    /// </summary>
    public string? UsersSpecification { get; set; } = null;

    /// <summary>
    ///     Filter specification restricting application of this filter to a selected roles only
    ///     If not present (or empty), this filter applies to all roles
    /// </summary>
    public string? RolesSpecification { get; set; } = null;

    public Embed Embed
    {
        get
        {
            var delay = Delay <= 0 ? "_This filter is applied immediately_" : $"Applied after `{Delay}` seconds";
            var pattern = Pattern == null ? "_This filter will match all messages_" : $"`{Pattern}`";
            var reposting = RepostChannelId == null
                ? "_This filter does not repost matched messages_"
                : $"<#{RepostChannelId}>";

            var users = DescribeFilterSpecificationString(UsersSpecification, e => $"<@{e}>");
            var roles = DescribeFilterSpecificationString(RolesSpecification, e => $"<@&{e}>");
            var channels = DescribeFilterSpecificationString(ChannelsSpecification, e => $"<#{e}>");

            return new EmbedBuilder()
                .WithColor(0x5865F2)
                .WithTitle($"Message filter #{Id}")
                .AddField("Delay before application", delay)
                .AddField("Regular expression", pattern)
                .AddField("Message reposting", reposting)
                .AddField("Users", users)
                .AddField("Roles", roles)
                .AddField("Channels", channels)
                .Build();
        }
    }

    private static string DescribeFilterSpecificationString(string? specificationString, Func<ulong, string> mapper)
    {
        var specification = new MessageFilterSpecification(specificationString);
        
        var explicitlyAllowedEntities = specification.AllowedEntities.Count > 0;
        var explicitlyDeniedEntities = specification.DeniedEntities.Count > 0;

        return (explicitlyAllowedEntities, explicitlyDeniedEntities) switch
        {
            (false, false) => "Applies without any limitations",
            (false, true) => "Applies to everything except " + string.Join(", ", specification.DeniedEntities.Select(mapper)),
            (true, _) => "Limited only to " + string.Join(", ", specification.AllowedEntities.Select(mapper))
        };
    }
}