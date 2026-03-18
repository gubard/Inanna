using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Inanna.Helpers;

public static class ApplicationExtension
{
    public static object? GetResourceOrNull(this Application app, string key)
    {
        app.TryGetResource(key, out var value);

        return value;
    }

    public static TopLevel? GetTopLevel(this Application app)
    {
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(x => x.IsFocused) ?? desktop.MainWindow;
        }

        if (app.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            return TopLevel.GetTopLevel(viewApp.MainView);
        }

        return null;
    }
}
