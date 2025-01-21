using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Commands;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using ServiceProvider = Beefweb.CommandLineTool.Services.ServiceProvider;

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
        using var serviceProvider = new ServiceProvider();

        var app = new CommandLineApplication<Program>();
        app.Conventions
            .UseDefaultConventions()
            .UseConstructorInjection(serviceProvider);

        return await app.ExecuteAsync(args);
    }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        application.ShowHelp();
        return Task.CompletedTask;
    }
}
