using System;
using System.IO;
using System.Text.Json;

namespace Beefweb.CommandLineTool.Services;

public interface ISettingsStorage
{
    Settings Settings { get; }

    void Save();
}

public sealed class SettingsStorage : ISettingsStorage
{
    private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

    private Settings? _settings;

    public Settings Settings => _settings ??= Load();

    private Settings Load()
    {
        var settingsFile = GetSettingsFile();

        if (!File.Exists(settingsFile))
        {
            return Settings.CreateDefault();
        }

        var settingsData = File.ReadAllBytes(settingsFile);
        return JsonSerializer.Deserialize<Settings>(settingsData, _serializerOptions) ?? Settings.CreateDefault();
    }

    public void Save()
    {
        var settings = Settings;
        var settingsFile = GetSettingsFile();
        var settingsDirectory = Path.GetDirectoryName(settingsFile);
        var tempFile = settingsFile + ".tmp";
        var settingsData = JsonSerializer.SerializeToUtf8Bytes(settings, _serializerOptions);

        Directory.CreateDirectory(settingsDirectory!);
        File.WriteAllBytes(tempFile, settingsData);
        File.Move(tempFile, settingsFile, true);
    }

    private static string GetSettingsFile()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Constants.ProjectName,
            Constants.AppName + ".config.json");
    }
}
