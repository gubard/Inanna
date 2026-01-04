namespace Inanna.Services;

public interface IRefresh
{
    ValueTask RefreshAsync(CancellationToken ct);
    void Refresh();
}

public interface IRefreshUi
{
    void RefreshUi();
}
