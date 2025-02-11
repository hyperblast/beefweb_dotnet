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
    IEnumerable<KeyValuePair<string, List<string>>> GetAllValues();

    List<string> GetValues(string name);

    void SetValues(string name, IEnumerable<string> values);
}

public class SettingsAccessor(ISettingsStorage storage) : ISettingsAccessor
{
    private static readonly FrozenDictionary<string, AccessorPair> Accessors;

    public IEnumerable<KeyValuePair<string, List<string>>> GetAllValues()
    {
        var settings = storage.Settings;
        return Accessors.Select(a => KeyValuePair.Create(a.Key, a.Value.reader.Invoke(settings)));
    }

    public List<string> GetValues(string name)
    {
        return GetAccessorPair(name).reader.Invoke(storage.Settings);
    }

    public void SetValues(string name, IEnumerable<string> values)
    {
        GetAccessorPair(name).writer.Invoke(storage.Settings, values);
    }

    private static AccessorPair GetAccessorPair(string name)
    {
        if (Accessors.TryGetValue(name, out var pair))
        {
            return pair;
        }

        throw new InvalidRequestException($"Unknown config property '{name}'.");
    }

    static SettingsAccessor()
    {
        var accessors = new Dictionary<string, AccessorPair>(StringComparer.OrdinalIgnoreCase)
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
            {
                nameof(Settings.PlayQueueFormat),
                (s => s.PlayQueueFormat, (s, v) => s.PlayQueueFormat = v.ToList())
            },
        };

        Accessors = accessors.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}
