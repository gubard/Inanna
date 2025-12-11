using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Helpers;

public static class TopLevelAssist
{
    public static readonly AvaloniaProperty<MaterialDesignSizeType> MaterialDesignSizeTypeProperty =
        AvaloniaProperty.RegisterAttached<TopLevel, MaterialDesignSizeType>(nameof(MaterialDesignSizeType),
            typeof(TopLevel));

    public static readonly AvaloniaProperty<bool> IsDraggingProperty =
        AvaloniaProperty.RegisterAttached<TopLevel, bool>(nameof(IDragAndDropService.IsDragging), typeof(TopLevel));

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
        if (triggerEvent.Source is not Visual source || source.GetVisualRoot() is not TopLevel topLevel)
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