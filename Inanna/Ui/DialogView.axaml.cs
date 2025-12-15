using Avalonia.Controls;

namespace Inanna.Ui;

public partial class DialogView : UserControl
{
    public DialogView()
    {
        InitializeComponent();
    }

    public DialogViewModel ViewModel =>
        DataContext as DialogViewModel ?? throw new NullReferenceException(nameof(DialogViewModel));
}
