using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Beefweb.CommandLineTool.Commands;
using Beefweb.CommandLineTool.Services;
using McMaster.Extensions.CommandLineUtils;
using ServiceProvider = Beefweb.CommandLineTool.Services.ServiceProvider;

namespace Beefweb.CommandLineTool;

[Command(Constants.AppName, Description = "Control music players in command line")]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
[Subcommand(typeof(PlayCommand))]
[Subcommand(typeof(StopCommand))]
[Subcommand(typeof(PauseCommand))]
[Subcommand(typeof(PositionCommand))]
[Subcommand(typeof(MuteCommand))]
[Subcommand(typeof(VolumeCommand))]
[Subcommand(typeof(ListCommand))]
[Subcommand(typeof(AddCommand))]
[Subcommand(typeof(CopyCommand))]
[Subcommand(typeof(MoveCommand))]
[Subcommand(typeof(DeleteCommand))]
[Subcommand(typeof(SortCommand))]
[Subcommand(typeof(ServersCommand))]
[Subcommand(typeof(PlaylistsCommand))]
[Subcommand(typeof(QueueCommand))]
[Subcommand(typeof(StatusCommand))]
[Subcommand(typeof(NowPlayingCommand))]
[Subcommand(typeof(ClientOptionsCommand))]
[Subcommand(typeof(OptionsCommand))]
[Subcommand(typeof(ListFileSystemCommand))]
public sealed class Program(CommandLineApplication application) : CommandBase
{
    private static string GetVersion()
        => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

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
            await Console.Error.WriteLineAsync(exception.Message);
            return 1;
        }
        catch (CommandParsingException exception)
        {
            await Console.Error.WriteLineAsync(exception.Message);
            return 1;
        }
        catch (InvalidRequestException exception)
        {
            await Console.Error.WriteLineAsync(exception.Message + " Try --help.");
            return 1;
        }
    }

    public override Task OnExecuteAsync(CancellationToken ct)
    {
        application.ShowHelp();
        return Task.CompletedTask;
    }
}
