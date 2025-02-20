using System.Collections.Generic;

namespace Beefweb.Client;

/// <summary>
/// Information about audio outputs.
/// </summary>
public sealed class OutputsInfo
{
    /// <summary>
    /// Information about active output device.
    /// </summary>
    public ActiveOutputInfo Active { get; set; } = null!;

    /// <summary>
    /// Available output device types.
    /// </summary>
    public IList<OutputTypeInfo> Types { get; set; } = null!;

    /// <summary>
    /// If true player supports multiple output types.
    /// </summary>
    public bool SupportsMultipleOutputTypes { get; set; }
}
