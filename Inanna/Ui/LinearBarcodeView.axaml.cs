using Avalonia.Controls;

namespace Inanna.Ui;

public sealed partial class LinearBarcodeView : UserControl
{
    public LinearBarcodeView()
    {
        InitializeComponent();
        Loaded += (_, _) => ViewModel.SetControl(this);
    }

    public LinearBarcodeViewModel ViewModel =>
        DataContext as LinearBarcodeViewModel
        ?? throw new NullReferenceException(nameof(LinearBarcodeViewModel));
}
