using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Helpers;

public interface IItemMutationService
{
    ConfiguredValueTaskAwaitable<ChangeOrderParameters<T>?> ShowChangeOrderAsync<T>(
        T[] items,
        T[] changeOrderItems,
        CancellationToken ct
    )
        where T : class, IOrderedItem;
}

public sealed class ItemMutationService : IItemMutationService
{
    public ItemMutationService(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IInannaViewModelFactory factory,
        ISafeExecuteWrapper safeExecuteWrapper,
        ICommandFactory commandFactory
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _factory = factory;
        _safeExecuteWrapper = safeExecuteWrapper;
        _commandFactory = commandFactory;
    }

    public ConfiguredValueTaskAwaitable<ChangeOrderParameters<T>?> ShowChangeOrderAsync<T>(
        T[] items,
        T[] changeOrderItems,
        CancellationToken ct
    )
        where T : class, IOrderedItem
    {
        return ShowChangeOrderCore(items, changeOrderItems, ct).ConfigureAwait(false);
    }

    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IInannaViewModelFactory _factory;
    private readonly ISafeExecuteWrapper _safeExecuteWrapper;
    private readonly ICommandFactory _commandFactory;

    private async ValueTask<ChangeOrderParameters<T>?> ShowChangeOrderCore<T>(
        T[] items,
        T[] changeOrderItems,
        CancellationToken ct
    )
        where T : class, IOrderedItem
    {
        foreach (var item in items)
        {
            item.IsChangingOrder = changeOrderItems.Contains(item);
        }

        var viewModel = _factory.CreateChangeOrder(items.OrderBy(x => x.OrderIndex));
        ChangeOrderParameters<T>? result = null;

        await _dialogService.ShowMessageBoxAsync(
            new(
                _appResourceService
                    .GetResource<string>("Lang.ChangeOrder")
                    .DispatchToDialogHeader(),
                viewModel,
                _safeExecuteWrapper,
                new(
                    _appResourceService.GetResource<string>("Lang.Ok"),
                    _commandFactory.CreateCommand(async c =>
                    {
                        result = new(viewModel.SelectedItem.Cast<T>(), viewModel.IsAfter);
                        await _dialogService.CloseMessageBoxAsync(c);
                    }),
                    null,
                    DialogButtonType.Primary
                ),
                _dialogService.CancelButton
            ),
            ct
        );

        return result;
    }
}
