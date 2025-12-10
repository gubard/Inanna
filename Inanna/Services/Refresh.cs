namespace Inanna.Services;

public interface IRefresh
{
    ValueTask RefreshAsync(CancellationToken ct);
    void Refresh();
}