using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Helpers;

namespace Inanna.Models;

public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors =>
        _isAnyExecute && _errors.Count != 0 && _errors.Any(x => x.Value.Invoke().Any());

    public void StartExecute()
    {
        _isAnyExecute = true;

        foreach (var error in _errors)
        {
            ErrorsChanged?.Invoke(this, new(error.Key));
        }
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (_isAnyExecute is false || propertyName is null)
        {
            return Enumerable.Empty<ValidationError>();
        }

        OnPropertyChanged(nameof(HasErrors));

        if (!_errors.TryGetValue(propertyName, out var validation))
        {
            return Enumerable.Empty<ValidationError>();
        }

        var errors = validation.Invoke();

        return errors;
    }

    protected ViewModelServices Services;

    protected ViewModelBase(ViewModelServices services)
    {
        Services = services;
    }

    protected void Post(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }

    protected void PostBackground(Action action)
    {
        Dispatcher.UIThread.Post(action, DispatcherPriority.Background);
    }

    protected void WrapCommand(Action action)
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        Services.SafeExecuteWrapper.Execute(action);
    }

    protected ConfiguredValueTaskAwaitable WrapCommandAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        StartExecute();

        return HasErrors
            ? TaskHelper.ConfiguredCompletedTask
            : Services.SafeExecuteWrapper.ExecuteAsync(func, ct);
    }

    protected ConfiguredValueTaskAwaitable WrapCommandAsync(
        Func<ValueTask> func,
        CancellationToken ct,
        bool isIgnoreErrors = false
    )
    {
        StartExecute();

        return HasErrors && !isIgnoreErrors
            ? TaskHelper.ConfiguredCompletedTask
            : Services.SafeExecuteWrapper.ExecuteAsync(
                () => func.Invoke().ConfigureAwait(false),
                ct
            );
    }

    protected ConfiguredValueTaskAwaitable WrapCommandAsync<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct,
        bool isBackground = false
    )
        where TValidationErrors : IValidationErrors
    {
        return WrapCommandCore(func, ct, isBackground).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable WrapCommandAsync<TValidationErrors>(
        Func<ValueTask<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        return HasErrors
            ? TaskHelper.ConfiguredCompletedTask
            : Services.SafeExecuteWrapper.ExecuteAsync(
                () => func.Invoke().ConfigureAwait(false),
                ct
            );
    }

    protected void WrapCommand<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        Services.SafeExecuteWrapper.Execute(func);
    }

    protected void SetValidation(string propertyName, Func<IEnumerable<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }

    protected ConfiguredValueTaskAwaitable<T?> ShowSelectItemAsync<T>(
        T[] items,
        CancellationToken ct
    )
    {
        return ShowSelectItemCore(items, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<T[]> ShowSelectItemsAsync<T>(
        T[] items,
        CancellationToken ct
    )
    {
        return ShowSelectItemsCore(items, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable ShowErrorAsync(
        ValidationError[] errors,
        CancellationToken ct
    )
    {
        return ShowErrorCore(errors, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable ShowErrorAsync(
        Exception[] exceptions,
        CancellationToken ct
    )
    {
        return ShowErrorCore(exceptions, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IStorageFile?> OpenFilePickerAsync(CancellationToken ct)
    {
        return OpenFilePickerCore(
                new()
                {
                    AllowMultiple = false,
                    Title = Services.AppResourceService.GetResource<string>("Lang.SelectFile"),
                },
                ct
            )
            .ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IStorageFolder?> OpenFolderPickerAsync(
        CancellationToken ct
    )
    {
        return OpenFolderPickerCore(
                new()
                {
                    AllowMultiple = false,
                    Title = Services.AppResourceService.GetResource<string>("Lang.SelectFolder"),
                },
                ct
            )
            .ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IStorageFile?> SaveFilePickerAsync(
        string defaultExtension,
        CancellationToken ct
    )
    {
        return SaveFilePickerCore(null, defaultExtension, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IStorageFile?> SaveFilePickerAsync(
        Uri suggestedStartLocation,
        string defaultExtension,
        CancellationToken ct
    )
    {
        return SaveFilePickerCore(suggestedStartLocation, defaultExtension, ct)
            .ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IReadOnlyList<IStorageFile>> OpenFilesPickerAsync(
        CancellationToken ct
    )
    {
        return OpenFilesPickerCore(null, ct).ConfigureAwait(false);
    }

    protected ConfiguredValueTaskAwaitable<IReadOnlyList<IStorageFile>> OpenFilesPickerAsync(
        Uri suggestedStartLocation,
        CancellationToken ct
    )
    {
        return OpenFilesPickerCore(suggestedStartLocation, ct).ConfigureAwait(false);
    }

    private bool _isAnyExecute;
    private readonly Dictionary<string, Func<IEnumerable<ValidationError>>> _errors = new();

    private async ValueTask<IStorageFile?> SaveFilePickerCore(
        Uri? suggestedStartLocation,
        string defaultExtension,
        CancellationToken ct
    )
    {
        var topLevel = Services.App.GetTopLevel();

        if (topLevel is null)
        {
            return null;
        }

        IStorageFolder? ssl = null;

        if (suggestedStartLocation is not null)
        {
            try
            {
                ssl = await topLevel
                    .StorageProvider.TryGetFolderFromPathAsync(suggestedStartLocation)
                    .WaitAsync(TimeSpan.FromSeconds(3), ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Services.Logger.GetFolderFromPathError(ex);

                ssl = await topLevel
                    .StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Desktop)
                    .ConfigureAwait(false);
            }
        }

        return await topLevel.StorageProvider.SaveFilePickerAsync(
            new()
            {
                Title = Services.AppResourceService.GetResource<string>("Lang.SaveFile"),
                DefaultExtension = defaultExtension,
                SuggestedStartLocation = ssl,
            }
        );
    }

    private async ValueTask<IStorageFolder?> OpenFolderPickerCore(
        FolderPickerOpenOptions options,
        CancellationToken ct
    )
    {
        var topLevel = Services.App.GetTopLevel();

        if (topLevel is null)
        {
            return null;
        }

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(options);

        if (folders.Count == 0)
        {
            return null;
        }

        return folders[0];
    }

    private async ValueTask<IStorageFile?> OpenFilePickerCore(
        FilePickerOpenOptions options,
        CancellationToken ct
    )
    {
        var topLevel = Services.App.GetTopLevel();

        if (topLevel is null)
        {
            return null;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);

        if (files.Count == 0)
        {
            return null;
        }

        return files[0];
    }

    private async ValueTask<IReadOnlyList<IStorageFile>> OpenFilesPickerCore(
        Uri? suggestedStartLocation,
        CancellationToken ct
    )
    {
        var storageProvider = Services.App.GetStorageProvider();

        if (storageProvider is null)
        {
            return [];
        }

        var ssl = suggestedStartLocation is not null
            ? await storageProvider
                .TryGetFolderFromPathAsync(suggestedStartLocation)
                .WaitAsync(TimeSpan.FromSeconds(3), ct)
                .ConfigureAwait(false)
            : null;

        return await storageProvider
            .OpenFilePickerAsync(
                new()
                {
                    AllowMultiple = true,
                    Title = Services.AppResourceService.GetResource<string>("Lang.SelectFiles"),
                    SuggestedStartLocation = ssl,
                }
            )
            .WaitAsync(TimeSpan.FromSeconds(3), ct)
            .ConfigureAwait(false);
    }

    private async ValueTask ShowErrorCore(Exception[] exceptions, CancellationToken ct)
    {
        await Services.DialogService.ShowMessageBoxAsync(
            Services.AppResourceService.GetResource<string>("Lang.Error"),
            Services.ErrorDialogFactory.Create(exceptions),
            ct,
            Services.DialogService.OkButton
        );
    }

    private async ValueTask ShowErrorCore(ValidationError[] errors, CancellationToken ct)
    {
        await Services.DialogService.ShowMessageBoxAsync(
            Services.AppResourceService.GetResource<string>("Lang.Error"),
            Services.ErrorDialogFactory.Create(errors),
            ct,
            Services.DialogService.OkButton
        );
    }

    private async ValueTask<T?> ShowSelectItemCore<T>(T[] items, CancellationToken ct)
    {
        var selectedItem = default(T);

        var list = await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var l = new ListBox
            {
                [ItemsControl.ItemsSourceProperty] = items,
                [ListBox.SelectionModeProperty] = SelectionMode.Single,
            };

            l.SelectionChanged += async (_, e) =>
            {
                selectedItem = e.AddedItems.OfType<T>().First();
                await Services.DialogService.CloseMessageBoxAsync(CancellationToken.None);
            };

            return l;
        });

        await Services.DialogService.ShowMessageBoxAsync(
            Services.AppResourceService.GetResource<string>("Lang.Select"),
            list,
            ct,
            Services.DialogService.CancelButton
        );

        return selectedItem;
    }

    private async ValueTask<T[]> ShowSelectItemsCore<T>(T[] items, CancellationToken ct)
    {
        var list = await Dispatcher.UIThread.InvokeAsync(() =>
            new ListBox
            {
                [ItemsControl.ItemsSourceProperty] = items,
                [ListBox.SelectionModeProperty] = SelectionMode.Multiple,
            }
        );

        await Services.DialogService.ShowMessageBoxAsync(
            Services.AppResourceService.GetResource<string>("Lang.Select"),
            list,
            ct,
            Services.DialogService.CreateButton(
                Services.AppResourceService.GetResource<string>("Lang.Ok"),
                async c => await Services.DialogService.CloseMessageBoxAsync(c),
                DialogButtonType.Primary
            )
        );

        return await Dispatcher.UIThread.InvokeAsync(() =>
            list.SelectedItems?.OfType<T>().ToArray() ?? []
        );
    }

    private async ValueTask WrapCommandCore<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct,
        bool isBackground = false
    )
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        var c = isBackground ? CancellationToken.None : ct;
        var task = Services.SafeExecuteWrapper.ExecuteAsync(func, c);

        if (isBackground)
        {
            return;
        }

        await task;
    }
}
