namespace Beefweb.Client;

/// <summary>
/// Information about active output device.
/// </summary>
public sealed class ActiveOutputInfo
{
    /// <summary>
    /// Output type type id.
    /// </summary>
    public string TypeId { get; set; } = null!;

    /// <summary>
    /// Output device id.
    /// </summary>
    public string DeviceId { get; set; } = null!;
}
