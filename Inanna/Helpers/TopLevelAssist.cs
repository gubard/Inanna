using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Helpers;

public static class TopLevelAssist
{
    static TopLevelAssist()
    {
        Layoutable.WidthProperty.Changed.AddClassHandler<TopLevel>((topLevel, _) =>
        {
            var currentType = topLevel.Width switch
            {
                <= MaterialDesign.MaxExtraSmall => MaterialDesignSizeType.ExtraSmall,
                > MaterialDesign.MaxExtraSmall and <= MaterialDesign.MaxSmall => MaterialDesignSizeType.Small,
                > MaterialDesign.MaxSmall and <= MaterialDesign.MaxMedium => MaterialDesignSizeType.Medium,
                > MaterialDesign.MaxMedium and <= MaterialDesign.MaxLarge => MaterialDesignSizeType.Large,
                > MaterialDesign.MaxLarge => MaterialDesignSizeType.ExtraLarge,
                _ => MaterialDesignSizeType.ExtraSmall,
            };

            var type = topLevel.GetValue<MaterialDesignSizeType>(MaterialDesignSizeTypeProperty);

            if (type != currentType)
            {
                topLevel.SetValue(MaterialDesignSizeTypeProperty, currentType);
                var pseudoClasses = (IPseudoClasses)topLevel.Classes;

                switch (currentType)
                {
                    case MaterialDesignSizeType.ExtraSmall:
                        pseudoClasses.Set(":extra-small", true);
                        pseudoClasses.Set(":small", false);
                        pseudoClasses.Set(":medium", false);
                        pseudoClasses.Set(":large", false);
                        pseudoClasses.Set(":extra-large", false);

                        break;
                    case MaterialDesignSizeType.Small:
                        pseudoClasses.Set(":extra-small", false);
                        pseudoClasses.Set(":small", true);
                        pseudoClasses.Set(":medium", false);
                        pseudoClasses.Set(":large", false);
                        pseudoClasses.Set(":extra-large", false);

                        break;
                    case MaterialDesignSizeType.Medium:
                        pseudoClasses.Set(":extra-small", false);
                        pseudoClasses.Set(":small", false);
                        pseudoClasses.Set(":medium", true);
                        pseudoClasses.Set(":large", false);
                        pseudoClasses.Set(":extra-large", false);

                        break;
                    case MaterialDesignSizeType.Large:
                        pseudoClasses.Set(":extra-small", false);
                        pseudoClasses.Set(":small", false);
                        pseudoClasses.Set(":medium", false);
                        pseudoClasses.Set(":large", true);
                        pseudoClasses.Set(":extra-large", false);

                        break;
                    case MaterialDesignSizeType.ExtraLarge:
                        pseudoClasses.Set(":extra-small", true);
                        pseudoClasses.Set(":small", false);
                        pseudoClasses.Set(":medium", false);
                        pseudoClasses.Set(":large", false);
                        pseudoClasses.Set(":extra-large", true);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentType), currentType, null);
                }
            }
        });
    }

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