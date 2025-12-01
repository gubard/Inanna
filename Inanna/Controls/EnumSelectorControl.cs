using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Inanna.Services;

namespace Inanna.Controls;

public class EnumSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<ValueType?> SelectedEnumProperty =
        AvaloniaProperty.Register<EnumSelectorControl, ValueType?>(
            nameof(SelectedEnum),
            defaultBindingMode: BindingMode.TwoWay
        );

    private SelectingItemsControl? _selectingItemsControl;

    static EnumSelectorControl()
    {
        SelectedEnumProperty.Changed.AddClassHandler<EnumSelectorControl>((control, _) => control.UpdateEnums());
    }

    public ValueType? SelectedEnum
    {
        get => GetValue(SelectedEnumProperty);
        set => SetValue(SelectedEnumProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        UpdateSelectingItemsControl(lb);
    }

    private void UpdateSelectingItemsControl(SelectingItemsControl? lb)
    {
        if (_selectingItemsControl is not null)
        {
            _selectingItemsControl.SelectionChanged -= OnSelectionChanged;
        }

        _selectingItemsControl = lb;

        if (_selectingItemsControl is not null)
        {
            _selectingItemsControl.SelectionChanged += OnSelectionChanged;

            _selectingItemsControl.DisplayMemberBinding = new Binding
            {
                Converter = EnumLocalizationValueConverter.Instance,
            };
        }

        UpdateEnums();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is not SelectingItemsControl lb)
        {
            return;
        }

        if (lb.SelectedItem is not ValueType vt)
        {
            return;
        }

        if (!Equals(SelectedEnum, vt))
        {
            SelectedEnum = vt;
        }
    }

    private void UpdateEnums()
    {
        if (_selectingItemsControl is null)
        {
            return;
        }

        if (SelectedEnum is null)
        {
            if (_selectingItemsControl.Items.Count != 0)
            {
                _selectingItemsControl.Items.Clear();
            }

            return;
        }

        var first = _selectingItemsControl.Items.FirstOrDefault();
        var enumType = SelectedEnum.GetType();

        if (first is null)
        {
            var values = Enum.GetValues(enumType);

            foreach (var value in values)
            {
                _selectingItemsControl.Items.Add(value);
            }

            _selectingItemsControl.SelectedItem = SelectedEnum;

            return;
        }

        if (first.GetType() != enumType)
        {
            _selectingItemsControl.Items.Clear();
            var values = Enum.GetValues(enumType);

            foreach (var value in values)
            {
                _selectingItemsControl.Items.Add(value);
            }

            _selectingItemsControl.SelectedItem = SelectedEnum;

            return;
        }

        if (!Equals(_selectingItemsControl.SelectedItem, SelectedEnum))
        {
            _selectingItemsControl.SelectedItem = SelectedEnum;
        }
    }
}