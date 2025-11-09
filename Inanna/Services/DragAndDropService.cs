using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Exceptions;

namespace Inanna.Services;

public interface IDragAndDropService
{
    object? GetDataAndRelease();
    void SetData(object? data);
    bool IsDragging { get; }
}

public partial class DragAndDropService : ObservableObject, IDragAndDropService
{
    private object? _data;
    [ObservableProperty] private bool _isDragging;

    public object? GetDataAndRelease()
    {
        if (!IsDragging)
        {
            throw new DataNotDraggingException();
        }

        var result = _data;
        _data = null;
        IsDragging = false;

        return result;
    }

    public void SetData(object? data)
    {
        if (IsDragging)
        {
            throw new DataDraggingException();
        }

        _data = data;
        IsDragging = true;
    }
}