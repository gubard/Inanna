using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Models;
using Gaia.Services;
using Inanna.Helpers;

namespace Inanna.Models;

public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    private bool _isAnyExecute;

    private readonly Dictionary<string, Func<IEnumerable<ValidationError>>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors =>
        _isAnyExecute && _errors.Count != 0 && _errors.Any(x => x.Value.Invoke().Any());

    protected void WrapCommand(Action action)
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        UiHelper.Execute(action);
    }

    protected ValueTask WrapCommand(Func<ValueTask> func)
    {
        StartExecute();

        return HasErrors ? ValueTask.CompletedTask : UiHelper.ExecuteAsync(func);
    }

    protected ValueTask WrapCommand<TValidationErrors>(Func<ValueTask<TValidationErrors>> func)
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        return HasErrors ? ValueTask.CompletedTask : UiHelper.ExecuteAsync(func);
    }

    protected void WrapCommand<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        StartExecute();

        if (HasErrors)
        {
            return;
        }

        UiHelper.Execute(func);
    }

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

    protected void SetValidation(string propertyName, Func<IEnumerable<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }
}
