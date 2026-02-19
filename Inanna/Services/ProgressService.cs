using System.ComponentModel;
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
        NeedValue += item.NeedValue;
        UpdateValues();
    }

    [ObservableProperty]
    private uint _needValue;

    [ObservableProperty]
    private uint _currentValue;

    private List<ProgressItem> _items = new();

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
        NeedValue -= (uint)removeItems.Sum(x => x.NeedValue);

        foreach (var item in removeItems)
        {
            item.PropertyChanged -= OnCurrentValueChanged;
            _items.Remove(item);
        }

        CurrentValue = (uint)_items.Sum(x => x.CurrentValue);
    }
}
