using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Inanna.Ui;

public partial class ChangeOrderViewModel : ViewModelBase
{
    public IEnumerable<IOrderedItem> Items => _items;

    public ChangeOrderViewModel(IEnumerable<IOrderedItem> items)
    {
        _items = new(items);
        _selectedItem = _items.First(x => !x.IsChangingOrder);
    }

    [ObservableProperty]
    private bool _isAfter;

    [ObservableProperty]
    private IOrderedItem _selectedItem;

    private readonly AvaloniaList<IOrderedItem> _items;
}
