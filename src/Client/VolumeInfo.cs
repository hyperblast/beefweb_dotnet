namespace Beefweb.Client
{
    /// <summary>
    /// Volume control information.
    /// </summary>
    public sealed class VolumeInfo
    {
        /// <summary>
        /// Volume type.
        /// </summary>
        public VolumeType Type { get; set; }

        /// <summary>
        /// Minimal volume value.
        /// </summary>
        public decimal Min { get; set; }

        /// <summary>
        /// Maximal volume value.
        /// </summary>
        public decimal Max { get; set; }

        /// <summary>
        /// Current volume value.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Is volume muted currently.
        /// </summary>
        public bool IsMuted { get; set; }
    }
}
