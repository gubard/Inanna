using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface ISaveUi
{
    ConfiguredValueTaskAwaitable SaveAsync(CancellationToken ct);
}
