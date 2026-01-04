using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface IRefresh
{
    ConfiguredValueTaskAwaitable RefreshAsync(CancellationToken ct);
    void Refresh();
}

public interface IRefreshUi
{
    void RefreshUi();
}
