using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using AccessorEntry = (
    bool isMultiValue,
    System.Func<Beefweb.CommandLineTool.Services.Settings, System.Collections.Generic.List<string>> reader,
    System.Action<Beefweb.CommandLineTool.Services.Settings, System.Collections.Generic.IEnumerable<string>> writer);

namespace Beefweb.CommandLineTool.Services;

public interface ISettingsAccessor
{
    IEnumerable<(string name, bool isMultiValue, List<string> values)> GetAllValues();

    List<string> GetValues(string name);

    void SetValues(string name, IEnumerable<string> values);

    void ResetValues(string name);
}

public class SettingsAccessor(ISettingsStorage storage) : ISettingsAccessor
{
    private static readonly FrozenDictionary<string, AccessorEntry> Accessors;

    public IEnumerable<(string name, bool isMultiValue, List<string> values)> GetAllValues()
    {
        var settings = storage.Settings;
        return Accessors.Select(a => (a.Key, a.Value.isMultiValue, a.Value.reader.Invoke(settings)));
    }

    public List<string> GetValues(string name)
    {
        return GetEntry(name).reader.Invoke(storage.Settings);
    }

    public void SetValues(string name, IEnumerable<string> values)
    {
        GetEntry(name).writer.Invoke(storage.Settings, values);
    }

    public void ResetValues(string name)
    {
        var accessor = GetEntry(name);
        var defaultSettings = Settings.CreateDefault();
        accessor.writer.Invoke(storage.Settings, accessor.reader.Invoke(defaultSettings));
    }

    private static AccessorEntry GetEntry(string name)
    {
        if (Accessors.TryGetValue(name, out var pair))
        {
            return pair;
        }

        throw new InvalidRequestException($"Unknown config property '{name}'.");
    }

    static SettingsAccessor()
    {
        var accessors = new Dictionary<string, AccessorEntry>(StringComparer.OrdinalIgnoreCase)
        {
            {
                nameof(Settings.NowPlayingFormat),
                (false, s => [s.NowPlayingFormat], (s, v) => s.NowPlayingFormat = v.First())
            },
            {
                nameof(Settings.StatusFormat),
                (false, s => [s.StatusFormat], (s, v) => s.StatusFormat = v.First())
            },
            {
                nameof(Settings.ListFormat),
                (true, s => s.ListFormat, (s, v) => s.ListFormat = v.ToList())
            },
            {
                nameof(Settings.PlayQueueFormat),
                (true, s => s.PlayQueueFormat, (s, v) => s.PlayQueueFormat = v.ToList())
            },
        };

        Accessors = accessors.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}
