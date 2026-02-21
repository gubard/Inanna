using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface IInit
{
    ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct);
}
