using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class DialogViewModel : ViewModelBase, ISave
{
    public DialogViewModel(
        object header,
        object content,
        ViewModelServices services,
        params Span<DialogButton> buttons
    )
        : base(services)
    {
        Content = content;
        Header = header;
        Buttons = new HashSet<DialogButton>(buttons.ToArray());
    }

    public object Header { get; }
    public object Content { get; }
    public IReadOnlySet<DialogButton> Buttons { get; }

    public ConfiguredValueTaskAwaitable SaveAsync(CancellationToken ct)
    {
        if (Content is ISave save)
        {
            return save.SaveAsync(ct);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }
}
