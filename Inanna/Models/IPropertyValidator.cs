using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Models;

namespace Inanna.Models;

public interface IPropertyValidator : INotifyDataErrorInfo
{
    void StartExecute();
}

public abstract class PropertyValidator : ObservableObject, IPropertyValidator
{
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors
    {
        get
        {
            if (!_isAnyExecute)
            {
                return false;
            }

            if (_errors.Count != 0 && _errors.Any(x => x.Value.Invoke().Any()))
            {
                return true;
            }

            return _assignedPropertyValidators.Any(x => x.HasErrors);
        }
    }

    public void StartExecute()
    {
        _isAnyExecute = true;

        foreach (var error in _errors)
        {
            ErrorsChanged?.Invoke(this, new(error.Key));
        }

        foreach (var assignedPropertyValidator in _assignedPropertyValidators)
        {
            assignedPropertyValidator.StartExecute();
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

    protected T AddAssignedPropertyValidator<T>(T assignedPropertyValidator)
        where T : IPropertyValidator
    {
        _assignedPropertyValidators.Add(assignedPropertyValidator);

        return assignedPropertyValidator;
    }

    protected void RemoveAssignedPropertyValidator(IPropertyValidator assignedPropertyValidator)
    {
        _assignedPropertyValidators.Remove(assignedPropertyValidator);
    }

    protected void SetValidation(string propertyName, Func<IEnumerable<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }

    private bool _isAnyExecute;
    private readonly Dictionary<string, Func<IEnumerable<ValidationError>>> _errors = new();
    private readonly List<IPropertyValidator> _assignedPropertyValidators = new();
}
