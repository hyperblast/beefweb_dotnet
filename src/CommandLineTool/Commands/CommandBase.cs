using System.Threading;
using System.Threading.Tasks;

namespace Beefweb.CommandLineTool.Commands;

public abstract class CommandBase
{
    public abstract Task OnExecuteAsync(CancellationToken ct);
}
