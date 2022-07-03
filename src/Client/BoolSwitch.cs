namespace Beefweb.Client
{
    /// <summary>
    /// Toggleable boolean value.
    /// </summary>
    public enum BoolSwitch
    {
        /// <summary>
        /// Request setting to transition to false value.
        /// </summary>
        False,

        /// <summary>
        /// Requests setting to transition to true value.
        /// </summary>
        True,

        /// <summary>
        /// Requests setting to toggle current value.
        /// False becomes true and vice versa.
        /// </summary>
        Toggle,
    }
}
