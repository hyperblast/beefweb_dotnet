using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Commands;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Beefweb.CommandLineTool;

[Command(Constants.AppName, Description = "Control music players in command line")]
[Subcommand(typeof(PlayCommand))]
[Subcommand(typeof(StopCommand))]
[Subcommand(typeof(AddServerCommand))]
[Subcommand(typeof(DeleteServerCommand))]
[Subcommand(typeof(ListServersCommand))]
[Subcommand(typeof(SetDefaultServerCommand))]
public sealed class Program(CommandLineApplication application) : CommandBase
{
    private static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();
        AddServices(services);
        await using var serviceProvider = services.BuildServiceProvider();

        var app = new CommandLineApplication<Program>();
        app.Conventions
            .UseDefaultConventions()
            .UseConstructorInjection(serviceProvider);

        return await app.ExecuteAsync(args);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IClientProvider, ClientProvider>();
        services.AddSingleton<ISettingsStorage, SettingsStorage>();
    }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        application.ShowHelp();
        return Task.CompletedTask;
    }
}
