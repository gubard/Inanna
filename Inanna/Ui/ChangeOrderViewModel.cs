using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class ChangeOrderViewModel : ViewModelBase
{
    public IEnumerable<IOrderedItem> Items => _items;

    public ChangeOrderViewModel(IEnumerable<IOrderedItem> items, ViewModelServices services)
        : base(services)
    {
        _isAfter = true;
        _items = new(items);
        _selectedItem = _items.First(x => !x.IsChangingOrder);
    }

    [ObservableProperty]
    private bool _isAfter;

    [ObservableProperty]
    private IOrderedItem _selectedItem;

    private readonly AvaloniaList<IOrderedItem> _items;
}
