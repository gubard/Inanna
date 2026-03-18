using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Inanna.Models;

namespace Inanna.Helpers;

public static class TopLevelAssist
{
    public static readonly AvaloniaProperty<MaterialDesignSizeType> MaterialDesignSizeTypeProperty =
        AvaloniaProperty.RegisterAttached<TopLevel, MaterialDesignSizeType>(
            nameof(MaterialDesignSizeType),
            typeof(TopLevel)
        );

    public static readonly AvaloniaProperty<bool> IsDraggingProperty =
        AvaloniaProperty.RegisterAttached<TopLevel, bool>("IsDragging", typeof(TopLevel));

    public static MaterialDesignSizeType GetMaterialDesignSizeType(TopLevel element)
    {
        return element.GetValue<MaterialDesignSizeType>(MaterialDesignSizeTypeProperty);
    }

    public static bool GetIsDragging(TopLevel element)
    {
        return element.GetValue<bool>(IsDraggingProperty);
    }

    public static async Task<DragDropEffects> DoDragDropAsync(
        PointerEventArgs triggerEvent,
        IDataTransfer dataTransfer,
        DragDropEffects allowedEffects
    )
    {
        if (triggerEvent.Source is not Visual source)
        {
            return await DragDrop.DoDragDropAsync(triggerEvent, dataTransfer, allowedEffects);
        }

        var topLevel = TopLevel.GetTopLevel(source);

        if (topLevel is null)
        {
            return await DragDrop.DoDragDropAsync(triggerEvent, dataTransfer, allowedEffects);
        }

        topLevel.SetValue(IsDraggingProperty, true);
        var pseudoClasses = (IPseudoClasses)topLevel.Classes;
        pseudoClasses.Set(":dragging", true);
        var effects = await DragDrop.DoDragDropAsync(triggerEvent, dataTransfer, allowedEffects);
        topLevel.SetValue(IsDraggingProperty, false);
        pseudoClasses.Set(":dragging", false);

        return effects;
    }
}
