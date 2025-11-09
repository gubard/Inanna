using Inanna.Models;

namespace Inanna.Ui;

public class DialogViewModel : ViewModelBase
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
}