namespace Beefweb.Client;

/// <summary>
/// Volume type.
/// </summary>
public enum VolumeType
{
    /// <summary>
    /// Volume type is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// Volume type is dB.
    /// </summary>
    Db,

    /// <summary>
    /// Volume type is linear.
    /// </summary>
    Linear,

    /// <summary>
    /// Similar to <see cref="Linear"/>,
    /// but only <see cref="IPlayerClient.VolumeUp"/> and <see cref="IPlayerClient.VolumeDown"/>
    /// methods are allowed to change volume.
    /// </summary>
    UpDown,
}
