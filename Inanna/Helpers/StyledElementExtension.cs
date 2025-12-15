using Avalonia;
using Avalonia.Controls;

namespace Inanna.Helpers;

public static class StyledElementExtension
{
    public static void SetPseudoClass<TStyledElement>(
        this TStyledElement styledElement,
        string pseudoClass,
        bool value
    )
        where TStyledElement : StyledElement
    {
        if (styledElement.Classes is IPseudoClasses classes)
        {
            classes.Set(pseudoClass, value);
        }
    }
}
