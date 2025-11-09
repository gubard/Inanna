using Avalonia;
using Avalonia.Controls;
using Inanna.Enums;

namespace Inanna.Controls;

public class NotificationControl : Button
{
    public static readonly StyledProperty<NotificationType> TypeProperty =
        AvaloniaProperty.Register<NotificationControl, NotificationType>(nameof(Type));

    public NotificationType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TypeProperty)
        {
            switch (Type)
            {
                case NotificationType.None:
                    PseudoClasses.Set(":info", false);
                    PseudoClasses.Set(":error", false);
                    PseudoClasses.Set(":progress", false);
                    PseudoClasses.Set(":success", false);
                    PseudoClasses.Set(":warning", false);
                    break;
                case NotificationType.Info:
                    PseudoClasses.Set(":info", true);
                    PseudoClasses.Set(":error", false);
                    PseudoClasses.Set(":progress", false);
                    PseudoClasses.Set(":success", false);
                    PseudoClasses.Set(":warning", false);
                    break;
                case NotificationType.Success:
                    PseudoClasses.Set(":info", false);
                    PseudoClasses.Set(":error", false);
                    PseudoClasses.Set(":progress", false);
                    PseudoClasses.Set(":success", true);
                    PseudoClasses.Set(":warning", false);
                    break;
                case NotificationType.Error:
                    PseudoClasses.Set(":info", false);
                    PseudoClasses.Set(":error", true);
                    PseudoClasses.Set(":progress", false);
                    PseudoClasses.Set(":success", false);
                    PseudoClasses.Set(":warning", false);
                    break;
                case NotificationType.Warning:
                    PseudoClasses.Set(":info", false);
                    PseudoClasses.Set(":error", false);
                    PseudoClasses.Set(":progress", false);
                    PseudoClasses.Set(":success", false);
                    PseudoClasses.Set(":warning", true);
                    break;
                case NotificationType.Progress:
                    PseudoClasses.Set(":info", false);
                    PseudoClasses.Set(":error", false);
                    PseudoClasses.Set(":progress", true);
                    PseudoClasses.Set(":success", false);
                    PseudoClasses.Set(":warning", false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}