using System;
using System.Collections.Generic;
using System.Linq;

namespace Beefweb.CommandLineTool.Services;

public sealed class ServiceProvider : IServiceProvider, IDisposable
{
    private readonly Dictionary<Type, object> _services;

    public ServiceProvider()
    {
        var settingsStorage = new SettingsStorage();
        var clientProvider = new ClientProvider(settingsStorage);

        _services = new Dictionary<Type, object>
        {
            { typeof(ISettingsStorage), settingsStorage },
            { typeof(IClientProvider), clientProvider }
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
