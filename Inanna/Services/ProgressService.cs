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
    private bool _isUpdating;

    private void OnCurrentValueChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProgressItem.CurrentValue))
        {
            UpdateValues();
        }
    }

    private void UpdateValues()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;
            var needValue = (uint)_items.Sum(x => x.NeedValue);
            var currentValue = (uint)_items.Sum(x => x.CurrentValue);

            if (currentValue >= needValue)
            {
                foreach (var item in _items)
                {
                    item.PropertyChanged -= OnCurrentValueChanged;
                }

                _items.Clear();
                NeedValue = 0;
                CurrentValue = 0;
            }
            else
            {
                NeedValue = needValue;
                CurrentValue = currentValue;
            }

            _isUpdating = false;
        });
    }
}
