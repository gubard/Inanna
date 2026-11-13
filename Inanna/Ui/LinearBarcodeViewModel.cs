using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;

namespace Inanna.Ui;

public sealed partial class LinearBarcodeViewModel : ViewModelBase
{
    public LinearBarcodeViewModel(ViewModelServices services)
        : base(services)
    {
        _barcode = new();
        BackgroundColor = Colors.White;
        BarColor = Colors.Black;
        BarWidth = 3;
        BarHeight = 80;
        BottomText = string.Empty;
        TopText = string.Empty;
    }

    public IEnumerable<bool> Barcode => _barcode;

    public Stream GetPngStream()
    {
        return _control.ThrowIfNull().ToPngStream();
    }

    public void SetControl(Control control)
    {
        _control = control;
    }

    public void SetBarcode(bool[] barcode)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _barcode.Clear();
            _barcode.AddRange(barcode);
        });
    }

    private readonly AvaloniaList<bool> _barcode;
    private Control? _control;

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    [ObservableProperty]
    public partial Color BarColor { get; set; }

    [ObservableProperty]
    public partial double BarWidth { get; set; }

    [ObservableProperty]
    public partial double BarHeight { get; set; }

    [ObservableProperty]
    public partial string BottomText { get; set; }

    [ObservableProperty]
    public partial string TopText { get; set; }
}
