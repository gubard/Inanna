using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Inanna.Models;

namespace Inanna.Services;

public sealed class LangResource : Styles, IResourceNode
{
    public static readonly DirectProperty<LangResource, Lang> LangProperty =
        AvaloniaProperty.RegisterDirect<LangResource, Lang>(
            nameof(Lang),
            o => o.Lang,
            (o, v) => o.Lang = v
        );

    public Lang Lang
    {
        get;
        set => SetAndRaise(LangProperty, ref field, value);
    }

    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        if (!base.TryGetResource(Lang, theme, out value))
        {
            return base.TryGetResource(key, theme, out value);
        }

        if (value is ResourceDictionary resourceDictionary)
        {
            return resourceDictionary.TryGetResource(key, theme, out value);
        }

        return base.TryGetResource(key, theme, out value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LangProperty)
        {
            Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Create());
        }
    }
}
