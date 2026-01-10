using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface IInitUi
{
    ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct);
}
