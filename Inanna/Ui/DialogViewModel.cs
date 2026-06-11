using Inanna.Models;

namespace Inanna.Ui;

public sealed class DialogViewModel : ViewModelBase
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
}
