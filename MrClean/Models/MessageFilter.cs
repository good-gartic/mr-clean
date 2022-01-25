namespace MrClean.Models;

public class MessageFilter
{
    public int Id { get; set; }

    /// <summary>
    ///     Delay in seconds, before the message is reposted / deleted
    /// </summary>
    public uint Delay { get; set; } = 0;

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
}