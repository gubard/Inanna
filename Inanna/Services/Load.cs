using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface ILoad
{
    ConfiguredValueTaskAwaitable LoadAsync(CancellationToken ct);
}
