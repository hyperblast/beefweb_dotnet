namespace Beefweb.Client;

/// <summary>
/// Type of <see cref="PlayerOption"/>
/// </summary>
public enum PlayerOptionType
{
    /// <summary>
    /// Invalid option type.
    /// </summary>
    Invalid,

    /// <summary>
    /// Player option is a enumeration.
    /// </summary>
    Enum,

    /// <summary>
    /// Player option is a switch.
    /// </summary>
    Bool,
}