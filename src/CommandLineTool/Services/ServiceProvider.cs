using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Beefweb.CommandLineTool.Services;

public sealed class ServiceProvider : IServiceProvider, IDisposable
{
    private readonly Dictionary<Type, object> _services;

    public ServiceProvider()
    {
        var console = PhysicalConsole.Singleton;
        var settingsStorage = new SettingsStorage();
        var settingsAccessor = new SettingsAccessor(settingsStorage);
        var clientProvider = new ClientProvider(settingsStorage);
        var tabularWriter = new TabularWriter(console);

        _services = new Dictionary<Type, object>
        {
            { typeof(ISettingsStorage), settingsStorage },
            { typeof(ISettingsAccessor), settingsAccessor },
            { typeof(IClientProvider), clientProvider },
            { typeof(ITabularWriter), tabularWriter }
        };
    }

    public object? GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);

    public void Dispose()
    {
        foreach (var disposable in _services.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }
}
