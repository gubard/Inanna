using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Layout;
using Inanna.Helpers;
using Inanna.Models;

namespace Inanna.Services;

public class InannaApplication : Application
{
    private static readonly HashSet<TopLevel> InitedTopLevels = new();

    static InannaApplication()
    {
        ToggleButton.IsCheckedProperty.OverrideMetadata<CheckBox>(
            new(false, BindingMode.TwoWay, enableDataValidation: true)
        );

        Layoutable.WidthProperty.Changed.AddClassHandler<TopLevel>(
            (topLevel, _) => UpdateMaterialDesignSizeType(topLevel)
        );

        InputElement.PointerPressedEvent.AddClassHandler<TreeViewItem>(
            (item, _) => item.IsExpanded = true
        );
    }

    protected void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    public static void UpdateMaterialDesignSizeType(TopLevel topLevel)
    {
        var currentType = topLevel.Width switch
        {
            <= MaterialDesign.MaxExtraSmall => MaterialDesignSizeType.ExtraSmall,
            > MaterialDesign.MaxExtraSmall and <= MaterialDesign.MaxSmall =>
                MaterialDesignSizeType.Small,
            > MaterialDesign.MaxSmall and <= MaterialDesign.MaxMedium =>
                MaterialDesignSizeType.Medium,
            > MaterialDesign.MaxMedium and <= MaterialDesign.MaxLarge =>
                MaterialDesignSizeType.Large,
            > MaterialDesign.MaxLarge => MaterialDesignSizeType.ExtraLarge,
            _ => MaterialDesignSizeType.ExtraSmall,
        };

        var type = topLevel.GetValue<MaterialDesignSizeType>(
            TopLevelAssist.MaterialDesignSizeTypeProperty
        );

        if (type == currentType && InitedTopLevels.Contains(topLevel))
        {
            return;
        }

        InitedTopLevels.Add(topLevel);
        topLevel.SetValue(TopLevelAssist.MaterialDesignSizeTypeProperty, currentType);
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
                pseudoClasses.Set(":extra-small", false);
                pseudoClasses.Set(":small", false);
                pseudoClasses.Set(":medium", false);
                pseudoClasses.Set(":large", false);
                pseudoClasses.Set(":extra-large", true);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentType), currentType, null);
        }
    }
}
