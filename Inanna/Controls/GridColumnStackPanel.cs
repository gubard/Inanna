using Avalonia;
using Avalonia.Controls;

namespace Inanna.Controls;

public class GridColumnStackPanel : Panel
{
    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<
        GridColumnStackPanel,
        int
    >(nameof(Columns), 1);

    private int _columns;

    static GridColumnStackPanel()
    {
        AffectsMeasure<GridColumnStackPanel>(ColumnsProperty);
    }

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateColumns();
        var maxWidth = 0d;
        var columnsHeight = new Dictionary<int, double>();

        for (var i = 0; i < _columns; i++)
        {
            columnsHeight[i] = 0;
        }

        var currentColumn = 0;

        var layoutSlotSize = new Size()
            .WithWidth(availableSize.Width / _columns)
            .WithHeight(double.PositiveInfinity);

        foreach (var child in Children)
        {
            child.Measure(layoutSlotSize);

            if (child.DesiredSize.Width > maxWidth)
            {
                maxWidth = child.DesiredSize.Width;
            }
        }

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }

            if (currentColumn >= _columns)
            {
                currentColumn = 0;
            }

            columnsHeight[currentColumn] += child.DesiredSize.Height;
            currentColumn++;
        }

        return new(maxWidth * _columns, columnsHeight.Values.Max());
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var columnsHeight = new Dictionary<int, double>();

        for (var i = 0; i < _columns; i++)
        {
            columnsHeight[i] = 0;
        }

        var x = 0;
        var maxWidth = 0d;
        var width = finalSize.Width / _columns;

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                child.Arrange(new(0, 0, 0, 0));

                continue;
            }

            if (x >= _columns)
            {
                x = 0;
            }

            if (child.DesiredSize.Width > maxWidth)
            {
                maxWidth = child.DesiredSize.Width;
            }

            child.Arrange(new(x * width, columnsHeight[x], width, child.DesiredSize.Height));
            columnsHeight[x] += child.DesiredSize.Height;
            x++;
        }

        return finalSize;
    }

    private void UpdateColumns()
    {
        _columns = Columns;
    }
}
