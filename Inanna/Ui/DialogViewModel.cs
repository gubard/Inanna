using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class DialogViewModel : ViewModelBase
{
    public DialogViewModel(
        object header,
        object content,
        ISafeExecuteWrapper safeExecuteWrapper,
        params Span<DialogButton> buttons
    )
        : base(safeExecuteWrapper)
    {
        Content = content;
        Header = header;
        Buttons = new HashSet<DialogButton>(buttons.ToArray());
    }

    public object Header { get; }
    public object Content { get; }
    public IReadOnlySet<DialogButton> Buttons { get; }
}
