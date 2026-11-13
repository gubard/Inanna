using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Inanna.Ui;

public sealed partial class ChangeOrderViewModel : ViewModelBase
{
    public IEnumerable<IOrderedItem> Items => _items;

    public ChangeOrderViewModel(IEnumerable<IOrderedItem> items, ViewModelServices services)
        : base(services)
    {
        IsAfter = true;
        _items = [.. items];
        SelectedItem = _items.First(x => !x.IsChangingOrder);
    }

    [ObservableProperty]
    public partial bool IsAfter { get; set; }

    [ObservableProperty]
    public partial IOrderedItem SelectedItem { get; set; }

    private readonly AvaloniaList<IOrderedItem> _items;
}
