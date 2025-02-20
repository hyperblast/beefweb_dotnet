using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Information about output device type.
/// </summary>
public sealed class OutputTypeInfo
{
    /// <summary>
    /// Output type id.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Output type name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Available output devices.
    /// </summary>
    public IList<OutputDeviceInfo> Devices { get; set; } = null!;
}
