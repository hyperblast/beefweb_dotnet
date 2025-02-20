namespace Beefweb.Client;

/// <summary>
/// Information about output device.
/// </summary>
public sealed class OutputDeviceInfo
{
    /// <summary>
    /// Output device id.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Output device name.
    /// </summary>
    public string Name { get; set; } = null!;
}
