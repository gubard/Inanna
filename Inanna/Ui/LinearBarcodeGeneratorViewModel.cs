using Avalonia.Collections;
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
        IInannaViewModelFactory viewModelFactory
    )
    {
        _text = string.Empty;
        _factory = factory;
        _barcodes = new(factory.SupportedBarcodes.ToArray());
        _selectedBarcode = _barcodes[0];
        Barcode = viewModelFactory.CreateLinearBarcode();
    }

    public IEnumerable<string> Barcodes => _barcodes;
    public LinearBarcodeViewModel Barcode { get; }

    [ObservableProperty]
    private string _text;

    [ObservableProperty]
    private string _selectedBarcode;

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
            Barcode.BottomText = Text;
            Barcode.TopText = SelectedBarcode;
            Barcode.BarWidth = serializer.BarWidth;
        });
    }
}
