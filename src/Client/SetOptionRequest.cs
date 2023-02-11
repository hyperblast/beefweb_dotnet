namespace Beefweb.Client
{
    /// <summary>
    /// Request to set option value.
    /// </summary>
    public class SetOptionRequest
    {
        /// <summary>
        /// Option id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// New option value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates request for enum option.
        /// </summary>
        /// <param name="id">Option id.</param>
        /// <param name="value">New option value.</param>
        public SetOptionRequest(string id, int value)
        {
            Id = id;
            Value = value;
        }

        /// <summary>
        /// Creates request for bool option.
        /// </summary>
        /// <param name="id">Option id.</param>
        /// <param name="value">New option value.</param>
        public SetOptionRequest(string id, bool value)
        {
            Id = id;
            Value = value;
        }

        /// <summary>
        /// Creates request for bool option.
        /// </summary>
        /// <param name="id">Option id.</param>
        /// <param name="value">New option value.</param>
        public SetOptionRequest(string id, BoolSwitch value)
        {
            Id = id;
            Value = value;
        }
    }
}
