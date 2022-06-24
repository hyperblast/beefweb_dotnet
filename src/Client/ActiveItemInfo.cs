using System;
using System.Collections.Generic;

namespace Beefweb.Client
{
    public sealed class ActiveItemInfo
    {
        public string PlaylistId { get; set; } = null!;

        public int PlaylistIndex { get; set; }

        public int Index { get; set; }

        public TimeSpan Position { get; set; }

        public TimeSpan Duration { get; set; }

        public IList<string> Columns { get; set; } = null!;
    }
}
