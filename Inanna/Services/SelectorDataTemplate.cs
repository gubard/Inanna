using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace Inanna.Services;

public sealed class SelectorDataTemplate : AvaloniaList<IDataTemplate>, ITreeDataTemplate
{
    public SelectorDataTemplate()
    {
        ResetBehavior = ResetBehavior.Remove;
    }

    public Control? Build(object? param)
    {
        return this.FirstOrDefault(x => x.Match(param))?.Build(param);
    }

    public bool Match(object? data)
    {
        return this.Any(x => x.Match(data));
    }

    class Disposable : IDisposable
    {
        public void Dispose() { }
    }

    public IDisposable BindChildren(
        AvaloniaObject target,
        AvaloniaProperty targetProperty,
        object item
    )
    {
        var template = this.OfType<TreeDataTemplate>().FirstOrDefault(x => x.Match(item));

        if (template is null)
        {
            return new Disposable();
        }

        return template.ItemsSource == null
            ? new Disposable()
            : target.Bind(targetProperty, template.ItemsSource);
    }
}
