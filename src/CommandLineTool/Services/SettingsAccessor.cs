using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using AccessorPair = (
    System.Func<Beefweb.CommandLineTool.Services.Settings, System.Collections.Generic.List<string>> reader,
    System.Action<Beefweb.CommandLineTool.Services.Settings, System.Collections.Generic.IEnumerable<string>> writer);

namespace Beefweb.CommandLineTool.Services;

public interface ISettingsAccessor
{
    List<string> GetValues(string name);

    void SetValues(string name, IEnumerable<string> values);
}

public class SettingsAccessor(ISettingsStorage storage) : ISettingsAccessor
{
    private static readonly FrozenDictionary<string, AccessorPair> Accessors;

    public List<string> GetValues(string name)
    {
        return GetAccessorPair(name).reader.Invoke(storage.Settings);
    }

    public void SetValues(string name, IEnumerable<string> values)
    {
        GetAccessorPair(name).writer.Invoke(storage.Settings, values);
    }

    private AccessorPair GetAccessorPair(string name)
    {
        if (Accessors.TryGetValue(name, out var pair))
        {
            return pair;
        }

        throw new InvalidRequestException($"Unknown config property '{name}'.");
    }

    static SettingsAccessor()
    {
        var accessors = new Dictionary<string, AccessorPair>
        {
            {
                nameof(Settings.NowPlayingFormat),
                (s => s.NowPlayingFormat, (s, v) => s.NowPlayingFormat = v.ToList())
            },
            {
                nameof(Settings.StatusFormat),
                (s => s.StatusFormat, (s, v) => s.StatusFormat = v.ToList())
            },
            {
                nameof(Settings.ListFormat),
                (s => s.ListFormat, (s, v) => s.ListFormat = v.ToList())
            },
        };

        Accessors = accessors.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}
