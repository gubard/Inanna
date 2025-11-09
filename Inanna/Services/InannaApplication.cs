using Avalonia;
using Avalonia.Data.Core.Plugins;
using Inanna.Helpers;

namespace Inanna.Services;

public class InannaApplication : Application
{
    static InannaApplication()
    {
        var property = TopLevelAssist.MaterialDesignSizeTypeProperty;
    }
    
    protected void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}