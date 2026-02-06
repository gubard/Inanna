using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface ILoadUi
{
    ConfiguredValueTaskAwaitable LoadUiAsync(CancellationToken ct);
}
