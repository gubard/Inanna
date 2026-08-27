using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Gaia.Models;

namespace Inanna.Models;

public interface IPropertyValidator : INotifyDataErrorInfo
{
    void StartExecute();
    IEnumerable<object> Errors { get; }
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

            if (_errors.Count != 0 && _errors.Any(x => !x.Value.Invoke().IsEmpty))
            {
                return true;
            }

            var result = _assignedPropertyValidators.Any(x => x.HasErrors);

            return result;
        }
    }

    public IEnumerable<object> Errors
    {
        get
        {
            if (!_isAnyExecute)
            {
                return [];
            }

            return    _errors.Select(x => x.Value.Invoke().Select(y=>(object)y)).Combine().Combine(_assignedPropertyValidators.Select(x=>x.Errors.ToArray().AsMemory())).ToArray();
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

        return errors.ToArray();
    }

    protected T AddAssignedPropertyValidator<T>(T assignedPropertyValidator)
        where T : IPropertyValidator
    {
        _assignedPropertyValidators.Add(assignedPropertyValidator);

        return assignedPropertyValidator;
    }

    protected void AddAssignedPropertyValidators<T>(T[] assignedPropertyValidators)
        where T : IPropertyValidator
    {
        _assignedPropertyValidators.AddRange(
            assignedPropertyValidators.OfType<IPropertyValidator>()
        );
    }

    protected void RemoveAssignedPropertyValidator(IPropertyValidator assignedPropertyValidator)
    {
        _assignedPropertyValidators.Remove(assignedPropertyValidator);
    }

    protected void SetValidation(string propertyName, Func<Memory<ValidationError>> validation)
    {
        _errors[propertyName] = validation;
    }

    private bool _isAnyExecute;
    private readonly Dictionary<string, Func<Memory<ValidationError>>> _errors = new();
    private readonly List<IPropertyValidator> _assignedPropertyValidators = new();
}
