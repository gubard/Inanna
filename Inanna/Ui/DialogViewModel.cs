using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class DialogViewModel : ViewModelBase, IInit, ILoadUi
{
    public DialogViewModel(object header, object content, params Span<DialogButton> buttons)
    {
        Content = content;
        Header = header;
        Buttons = new HashSet<DialogButton>(buttons.ToArray());
    }

    public object Header { get; }
    public object Content { get; }
    public IReadOnlySet<DialogButton> Buttons { get; }

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        if (Content is IInit initUi)
        {
            return initUi.InitAsync(ct);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable LoadUiAsync(CancellationToken ct)
    {
        if (Content is ILoadUi loadUi)
        {
            return loadUi.LoadUiAsync(ct);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }
}
