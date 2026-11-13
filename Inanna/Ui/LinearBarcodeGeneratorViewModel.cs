using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class LinearBarcodeGeneratorViewModel : ViewModelBase
{
    public LinearBarcodeGeneratorViewModel(
        ILinearBarcodeSerializerFactory factory,
        IInannaViewModelFactory viewModelFactory,
        ViewModelServices services
    )
        : base(services)
    {
        Text = string.Empty;
        _factory = factory;
        _barcodes = new(factory.SupportedBarcodes.ToArray());
        SelectedBarcode = _barcodes[0];
        Barcode = viewModelFactory.CreateLinearBarcode();
        IsShowBottomText = true;
        IsShowTopText = true;
    }

    public IEnumerable<string> Barcodes => _barcodes;
    public LinearBarcodeViewModel Barcode { get; }

    [ObservableProperty]
    public partial string Text { get; set; }

    [ObservableProperty]
    public partial string SelectedBarcode { get; set; }

    [ObservableProperty]
    public partial bool IsShowBottomText { get; set; }

    [ObservableProperty]
    public partial bool IsShowTopText { get; set; }

    private readonly AvaloniaList<string> _barcodes;
    private readonly ILinearBarcodeSerializerFactory _factory;

    [RelayCommand]
    private void GenerateBarcode()
    {
        WrapCommand(() =>
        {
            var serializer = _factory.Create(SelectedBarcode);
            var bytes = serializer.Serialize(Text);
            Barcode.SetBarcode(bytes.ToArray());

            Dispatcher.UIThread.Post(() =>
            {
                Barcode.BottomText = IsShowBottomText ? Text : string.Empty;
                Barcode.TopText = IsShowTopText ? SelectedBarcode : string.Empty;
                Barcode.BarWidth = serializer.BarWidth;
            });
        });
    }
}
