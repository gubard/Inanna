using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface IRefresh
{
    ConfiguredValueTaskAwaitable RefreshAsync(CancellationToken ct);
}

public interface IRefreshUi
{
    void RefreshUi();
}
