using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Inanna.Controls;

public class IntegersSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IList<int>?> SelectedIntegersProperty =
        AvaloniaProperty.Register<IntegersSelectorControl, IList<int>?>(nameof(SelectedIntegers));

    public static readonly StyledProperty<int> MinProperty = AvaloniaProperty.Register<
        IntegersSelectorControl,
        int
    >(nameof(Min));

    public static readonly StyledProperty<int> MaxProperty = AvaloniaProperty.Register<
        IntegersSelectorControl,
        int
    >(nameof(Max));

    private readonly List<IntegerSelectorItemControl> _integerControls = new();
    private ItemsControl? _itemsControl;
    private IList<int>? _selectedIntegers;

    public IList<int>? SelectedIntegers
    {
        get => GetValue(SelectedIntegersProperty);
        set => SetValue(SelectedIntegersProperty, value);
    }

    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public int Min
    {
        get => GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MaxProperty || change.Property == MinProperty)
        {
            UpdateIntegers();
        }
        else if (change.Property == SelectedIntegersProperty)
        {
            UpdateSelectedIntegers();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsControl = e.NameScope.Find<ItemsControl>("PART_ItemsControl");
        UpdateIntegers();
    }

    private void UpdateSelectedIntegers()
    {
        if (SelectedIntegers is null)
        {
            if (_selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= SelectedIntegersChangedEventHandler;
            }

            _selectedIntegers = null;

            foreach (var integerControl in _integerControls)
            {
                if (integerControl.IsSelected)
                {
                    integerControl.IsSelected = false;
                }
            }
        }
        else
        {
            if (!Equals(_selectedIntegers, SelectedIntegers))
            {
                if (_selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -=
                        SelectedIntegersChangedEventHandler;
                }

                _selectedIntegers = SelectedIntegers;

                if (_selectedIntegers is INotifyCollectionChanged changed)
                {
                    changed.CollectionChanged += SelectedIntegersChangedEventHandler;
                }
            }

            foreach (var integerControl in _integerControls)
            {
                var value = SelectedIntegers.Contains(integerControl.Value);

                if (value != integerControl.IsSelected)
                {
                    integerControl.IsSelected = value;
                }
            }
        }
    }

    private void SelectedIntegersChangedEventHandler(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        if (sender is not IEnumerable<int> enumerable)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                break;
            case NotifyCollectionChangedAction.Remove:
                break;
            case NotifyCollectionChangedAction.Replace:
                return;
            case NotifyCollectionChangedAction.Move:
                return;
            case NotifyCollectionChangedAction.Reset:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var integerControl in _integerControls)
        {
            var value = enumerable.Contains(integerControl.Value);

            if (value != integerControl.IsSelected)
            {
                integerControl.IsSelected = value;
            }
        }
    }

    private void Clear()
    {
        foreach (var integerControl in _integerControls)
        {
            integerControl.PropertyChanged -= OnPropertyChanged;
        }

        _integerControls.Clear();
        _itemsControl?.Items.Clear();
    }

    private void UpdateIntegers()
    {
        if (_itemsControl is null)
        {
            return;
        }

        Clear();

        if (Min > Max)
        {
            return;
        }

        _integerControls.AddRange(
            Enumerable.Range(Min, Max).Select(x => new IntegerSelectorItemControl { Value = x })
        );

        foreach (var integerControl in _integerControls)
        {
            integerControl.PropertyChanged += OnPropertyChanged;
        }

        foreach (var integer in _integerControls)
        {
            _itemsControl.Items.Add(integer);
        }

        UpdateSelectedIntegers();
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not IntegerSelectorItemControl integerControl)
        {
            return;
        }

        if (e.Property.Name == nameof(IntegerSelectorItemControl.IsSelected))
        {
            if (_selectedIntegers is null)
            {
                return;
            }

            if (integerControl.IsSelected)
            {
                if (_selectedIntegers.Contains(integerControl.Value))
                {
                    return;
                }

                if (_selectedIntegers is ICollection<int> collection)
                {
                    collection.Add(integerControl.Value);
                }
            }
            else
            {
                if (!_selectedIntegers.Contains(integerControl.Value))
                {
                    return;
                }

                if (_selectedIntegers is ICollection<int> collection)
                {
                    collection.Remove(integerControl.Value);
                }
            }
        }
    }
}
