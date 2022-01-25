namespace MrClean.Configuration;

#nullable disable
public class DiscordOptions
{
    public const string Section = "Discord";

    /// <summary>
    ///     Token used for the gateway authentication
    /// </summary>
    public string Token { get; set; }
}