using System.ComponentModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Inanna.Services;

public interface IProgressService
{
    uint NeedValue { get; }
    uint CurrentValue { get; }

    void AddProgress(ProgressItem item);
}

public sealed partial class ProgressService : ObservableObject, IProgressService
{
    public void AddProgress(ProgressItem item)
    {
        _items.Add(item);
        item.PropertyChanged += OnCurrentValueChanged;
        UpdateValues();
    }

    [ObservableProperty]
    private uint _needValue;

    [ObservableProperty]
    private uint _currentValue;

    private readonly List<ProgressItem> _items = new();

    private void OnCurrentValueChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProgressItem.CurrentValue))
        {
            UpdateValues();
        }
    }

    private void UpdateValues()
    {
        var removeItems = _items.Where(x => x.CurrentValue >= x.NeedValue).ToArray();

        foreach (var item in removeItems)
        {
            item.PropertyChanged -= OnCurrentValueChanged;
            _items.Remove(item);
        }

        Dispatcher.UIThread.Post(() =>
        {
            NeedValue = (uint)_items.Sum(x => x.NeedValue);
            CurrentValue = (uint)_items.Sum(x => x.CurrentValue);
        });
    }
}
