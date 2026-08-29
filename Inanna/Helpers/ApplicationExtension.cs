using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

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
        return Dispatcher.UIThread.Invoke(app.GetTopLevelCore, DispatcherPriority.Background);
    }

    private static TopLevel? GetTopLevelCore(this Application app)
    {
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.Windows.FirstOrDefault(x => x.IsFocused) ?? desktop.MainWindow;

            return window;
        }

        if (app.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            var view = TopLevel.GetTopLevel(viewApp.MainView);

            return view;
        }

        return null;
    }

    public static IStorageProvider? GetStorageProvider(this Application app)
    {
        return Dispatcher.UIThread.Invoke(
            () => app.GetTopLevelCore()?.StorageProvider,
            DispatcherPriority.Background
        );
    }
}
