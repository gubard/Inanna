using System.Collections;
using System.Collections.Frozen;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Gaia.Models;
using Inanna.Helpers;

namespace Inanna.Controls;

public sealed class DaysOfYearSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<DayOfYear>?> ItemsSourceProperty =
        AvaloniaProperty.Register<DaysOfYearSelectorControl, IEnumerable<DayOfYear>?>(
            nameof(ItemsSource));

    public static readonly DirectProperty<DaysOfYearSelectorControl, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<DaysOfYearSelectorControl, IEnumerable>(
            nameof(Items), x => x._items);

    private readonly FrozenDictionary<Month, IntegersSelectorControl> _items;
    private IAvaloniaList<DayOfYear>? _list;

    public DaysOfYearSelectorControl()
    {
        var items = new Dictionary<Month, IntegersSelectorControl>();
        var months = Enum.GetValues<Month>();

        for (var index = 0; index < months.Length; index++)
        {
            var month = months[index];
            var list = new AvaloniaList<int>();

            list.CollectionChanged += (_, e) =>
            {
                if (_list is null)
                {
                    return;
                }

                if (_items is null)
                {
                    return;
                }

                var newList = new List<DayOfYear>();

                foreach (var (m, selector) in _items)
                {
                    if (selector.SelectedIntegers is null)
                    {
                        continue;
                    }

                    foreach (var integer in selector.SelectedIntegers)
                    {
                        newList.Add(new((byte)integer, m));
                    }
                }

                _list.UpdateOrder(newList.ToArray());
            };

            items[month] = new()
            {
                Min = 1,
                Max = DateTime.DaysInMonth(4, (int)month),
                SelectedIntegers = list,
            };
        }

        _items = items.ToFrozenDictionary();
    }

    public IEnumerable Items => _items;

    public IEnumerable<DayOfYear>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            UpdateNotifyCollectionChanged();
            UpdateItems();
        }
    }

    private void UpdateNotifyCollectionChanged()
    {
        if (_list is not null)
        {
            _list.CollectionChanged -= OnCollectionChanged;
        }

        if (ItemsSource is IAvaloniaList<DayOfYear> notifyCollectionChanged)
        {
            _list = notifyCollectionChanged;
            _list.CollectionChanged += OnCollectionChanged;
        }
        else
        {
            _list = null;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        if (ItemsSource is null)
        {
            return;
        }

        var items = ItemsSource.GroupBy(x => x.Month).ToDictionary(x => x.Key);

        foreach (var (month, days) in items)
        {
            (_items[month].SelectedIntegers as AvaloniaList<int>)?.UpdateOrder(days.Select(x => (int)x.Day).ToArray());
        }
    }
}