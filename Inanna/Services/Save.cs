using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface ISave
{
    ConfiguredValueTaskAwaitable SaveAsync(CancellationToken ct);
}
