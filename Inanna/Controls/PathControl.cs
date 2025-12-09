using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using IconPacks.Avalonia.MaterialDesign;

namespace Inanna.Controls;

public class PathControl : TemplatedControl
{
    private static readonly FuncTemplate<Control?> DefaultSeparator = new(() =>
        new PackIconMaterialDesign
        {
            Kind = PackIconMaterialDesignKind.ChevronRight,
        });

    public static readonly StyledProperty<ITemplate<Control?>>
        SeparatorProperty =
            AvaloniaProperty.Register<PathControl, ITemplate<Control?>>(
                nameof(SeparatorPanel), DefaultSeparator);

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        ItemsControl.ItemTemplateProperty.AddOwner<PathControl>();

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<PathControl, IEnumerable?>(
            nameof(ItemsSource));

    public static readonly DirectProperty<PathControl, IList?> ItemsProperty =
        AvaloniaProperty.RegisterDirect<PathControl, IList?>(nameof(Items),
            control => control._items);

    private INotifyCollectionChanged? _notifyCollectionChanged;
    private IList? _items;

    static PathControl()
    {
        ItemsSourceProperty.Changed.AddClassHandler<PathControl>(
            (pc, _) =>
            {
                pc.UpdateDefaultItems();
                pc.UpdateNotifyCollectionChanged();
            }
        );
    }

    public PathControl()
    {
        Items = Array.Empty<object>();
    }

    public ITemplate<Control?> SeparatorPanel
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    public IList? Items
    {
        get => _items;
        private set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateDefaultItems();
    }

    private void UpdateNotifyCollectionChanged()
    {
        if (_notifyCollectionChanged is not null)
        {
            _notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
        }

        if (ItemsSource is INotifyCollectionChanged n)
        {
            _notifyCollectionChanged = n;
            _notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
        }
        else
        {
            _notifyCollectionChanged = null;
        }
    }

    private void OnCollectionChanged(object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        UpdateDefaultItems();
    }

    private void UpdateDefaultItems()
    {
        if (ItemsSource is null)
        {
            Items = null;

            return;
        }

        var segments = ItemsSource.Cast<object>().ToArray();

        if (segments.Length == 0)
        {
            Items = Array.Empty<object>();

            return;
        }

        if (segments.Length == 1)
        {
            Items = segments.ToArray();

            return;
        }

        var body = segments[..^1];
        var result = new object[segments.Length * 2 - 1].AsSpan();

        for (var i = 0; i < body.Length; i++)
        {
            result[i * 2] = body[i];
            result[i * 2 + 1] = SeparatorPanel.Build() ?? new object();
        }

        result[^1] = segments[^1];
        Items = result.ToArray();
    }
}