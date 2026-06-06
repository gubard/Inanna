using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Services;

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

    protected readonly ISafeExecuteWrapper SafeExecuteWrapper;

    protected ViewModelBase(ISafeExecuteWrapper safeExecuteWrapper)
    {
        SafeExecuteWrapper = safeExecuteWrapper;
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

        SafeExecuteWrapper.Execute(action);
    }

    protected ConfiguredValueTaskAwaitable WrapCommandAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        StartExecute();

        return HasErrors
            ? TaskHelper.ConfiguredCompletedTask
            : SafeExecuteWrapper.ExecuteAsync(func, ct);
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
            : SafeExecuteWrapper.ExecuteAsync(() => func.Invoke().ConfigureAwait(false), ct);
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
            : SafeExecuteWrapper.ExecuteAsync(() => func.Invoke().ConfigureAwait(false), ct);
    }

    protected void WrapCommand<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        SafeExecuteWrapper.Execute(func);
    }

    protected void SetValidation(string propertyName, Func<IEnumerable<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }

    private bool _isAnyExecute;
    private readonly Dictionary<string, Func<IEnumerable<ValidationError>>> _errors = new();

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
        var task = SafeExecuteWrapper.ExecuteAsync(func, c);

        if (isBackground)
        {
            return;
        }

        await task;
    }
}
