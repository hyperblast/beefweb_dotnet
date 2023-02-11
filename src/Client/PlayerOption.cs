using System.Collections.Generic;

namespace Beefweb.Client
{
    /// <summary>
    /// Configurable player option.
    /// </summary>
    public class PlayerOption
    {
        /// <summary>
        /// Option identifier.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Option display name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Option type.
        /// </summary>
        public PlayerOptionType Type { get; set; }

        /// <summary>
        /// Current option value.
        /// Depending on <see cref="Type"/> this value is either <see cref="bool"/> or <see cref="int"/>.
        /// </summary>
        public object Value { get; set; } = null!;

        /// <summary>
        /// List of enum option names.
        /// This collection is not null only if <see cref="Type"/> is <see cref="PlayerOptionType.Enum"/>.
        /// </summary>
        public IList<string>? EnumNames { get; set; }
    }
}
