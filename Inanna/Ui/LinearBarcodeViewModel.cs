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
    public LinearBarcodeViewModel()
    {
        _barcode = new();
        _backgroundColor = Colors.White;
        _barColor = Colors.Black;
        _barWidth = 3;
        _barHeight = 80;
        _bottomText = string.Empty;
        _topText = string.Empty;
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
    private Color _backgroundColor;

    [ObservableProperty]
    private Color _barColor;

    [ObservableProperty]
    private double _barWidth;

    [ObservableProperty]
    private double _barHeight;

    [ObservableProperty]
    private string _bottomText;

    [ObservableProperty]
    private string _topText;
}
