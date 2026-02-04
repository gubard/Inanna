using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Inanna.Models;

namespace Inanna.Helpers;

public static class InputElementAssist
{
    public static readonly AttachedProperty<bool> IsDragCredentialNotifyHandler =
        AvaloniaProperty.RegisterAttached<InputElement, bool>(
            nameof(IsDragCredentialNotifyHandler),
            typeof(InputElementAssist)
        );

    public static void SetIsDragHandle(InputElement element, bool value)
    {
        element.SetValue(IsDragCredentialNotifyHandler, value);
    }

    public static bool GetIsDragHandle(InputElement element)
    {
        return element.GetValue(IsDragCredentialNotifyHandler);
    }

    static InputElementAssist()
    {
        IsDragCredentialNotifyHandler.Changed.AddClassHandler<Control, bool>(
            (_, e) =>
            {
                if (e.Sender is not InputElement element)
                {
                    return;
                }

                if (e.NewValue.GetValueOrDefault<bool>())
                {
                    element.PointerPressed += DragOnPointerPressed;
                }
                else
                {
                    element.PointerPressed -= DragOnPointerPressed;
                }
            }
        );
    }

    private static async void DragOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not IIsDrag isDrag)
        {
            return;
        }

        e.Handled = true;
        var dragData = new DataTransfer();
        var dataTransferItem = new DataTransferItem();

        dataTransferItem.Set(
            DataFormat.CreateBytesApplicationFormat($"{isDrag.GetType().FullName}"),
            isDrag.Id.ToByteArray()
        );

        dragData.Add(dataTransferItem);
        isDrag.IsDrag = true;
        await TopLevelAssist.DoDragDropAsync(e, dragData, DragDropEffects.Move);
        isDrag.IsDrag = false;
    }
}
