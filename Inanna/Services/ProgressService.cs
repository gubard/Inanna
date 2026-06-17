using System.ComponentModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Inanna.Models;

namespace Inanna.Services;

public interface IProgressService
{
    uint NeedValue { get; }
    uint CurrentValue { get; }
    string Status { get; }

    void AddProgress(ProgressItem item);
}

public sealed partial class ProgressService : ObservableObject, IProgressService
{
    public void AddProgress(ProgressItem item)
    {
        _items.Add(item);
        item.PropertyChanged += OnCurrentValueChanged;
        UpdateValues();
        Status = _items
            .Select(x => x.Status)
            .Where(x => !x.IsNullOrWhiteSpace())
            .JoinString(Environment.NewLine);
    }

    [ObservableProperty]
    private uint _needValue;

    [ObservableProperty]
    private uint _currentValue;

    [ObservableProperty]
    private string _status = string.Empty;

    private readonly List<ProgressItem> _items = new();
    private bool _isUpdating;

    private void OnCurrentValueChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ProgressItem.CurrentValue):
                UpdateValues();
                break;

            case nameof(ProgressItem.Status):
                Status = _items
                    .Select(x => x.Status)
                    .Where(x => !x.IsNullOrWhiteSpace())
                    .JoinString(Environment.NewLine);
                break;
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
