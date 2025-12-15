using Inanna.Models;

namespace Inanna.Ui;

public abstract class ParametersViewModelBase : ViewModelBase
{
    protected readonly ValidationMode ValidationMode;

    protected ParametersViewModelBase(ValidationMode validationMode, bool isShowEdit)
    {
        ValidationMode = validationMode;
        IsShowEdit = isShowEdit;
    }

    public bool IsShowEdit { get; }
}
