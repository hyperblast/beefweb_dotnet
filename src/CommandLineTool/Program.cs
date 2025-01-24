using System;
using System.Net.Http;
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
[Subcommand(typeof(PauseCommand))]
[Subcommand(typeof(MuteCommand))]
[Subcommand(typeof(ServersCommand))]
[Subcommand(typeof(SetVolumeCommand))]
[Subcommand(typeof(PlaylistsCommand))]
[Subcommand(typeof(StatusCommand))]
[Subcommand(typeof(NowPlayingCommand))]
public sealed class Program(CommandLineApplication application) : CommandBase
{
    private static async Task<int> Main(string[] args)
    {
        using var serviceProvider = new ServiceProvider();

        var app = new CommandLineApplication<Program>();

        app.Conventions
            .UseDefaultConventions()
            .UseConstructorInjection(serviceProvider);

        app.ValueParsers.Add(new PlaylistRefParser());

        try
        {
            return await app.ExecuteAsync(args);
        }
        catch (HttpRequestException exception)
        {
            return WriteError(exception);
        }
        catch (InvalidRequestException exception)
        {
            return WriteError(exception);
        }
    }

    private static int WriteError(Exception exception)
    {
        Console.Error.WriteLine(exception.Message);
        return 1;
    }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        application.ShowHelp();
        return Task.CompletedTask;
    }
}
