using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Inanna.Controls;

public class IntegerSelectorItemControl : TemplatedControl
{
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<IntegerSelectorItemControl, int>(nameof(Value));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<IntegerSelectorItemControl, bool>(nameof(IsSelected));

    private ToggleButton? _toggleButton;

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != IsSelectedProperty)
        {
            return;
        }

        if (_toggleButton is null)
        {
            return;
        }

        if (_toggleButton.IsChecked != IsSelected)
        {
            _toggleButton.IsChecked = IsSelected;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var tb = e.NameScope.Find<ToggleButton>("PART_ToggleButton");
        UpdateToggleButton(tb);
    }

    private void UpdateToggleButton(ToggleButton? newToggleButton)
    {
        if (_toggleButton is not null)
        {
            _toggleButton.IsCheckedChanged -= OnIsCheckedChanged;
        }

        _toggleButton = newToggleButton;

        if (_toggleButton is null)
        {
            return;
        }

        _toggleButton.IsChecked = IsSelected;
        _toggleButton.IsCheckedChanged += OnIsCheckedChanged;
    }

    private void OnIsCheckedChanged(object? sender, RoutedEventArgs args)
    {
        if (sender is not ToggleButton tb)
        {
            return;
        }

        var value = tb.IsChecked.GetValueOrDefault();

        if (IsSelected != value)
        {
            IsSelected = tb.IsChecked.GetValueOrDefault();
        }
    }
}