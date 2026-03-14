using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public abstract class ParametersViewModelBase : ViewModelBase
{
    protected readonly ValidationMode ValidationMode;

    protected ParametersViewModelBase(
        ValidationMode validationMode,
        bool isShowEdit,
        ISafeExecuteWrapper safeExecuteWrapper
    )
        : base(safeExecuteWrapper)
    {
        ValidationMode = validationMode;
        IsShowEdit = isShowEdit;
    }

    public bool IsShowEdit { get; }
}
