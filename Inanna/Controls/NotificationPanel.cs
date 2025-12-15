using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Inanna.Models;

namespace Inanna.Controls;

public class NotificationPanel : ContentControl
{
    private static readonly Dictionary<string, NotificationPanel> NotificationPanels = new();

    public static readonly StyledProperty<IList?> TopLeftNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(TopLeftNotifications));

    public static readonly StyledProperty<IList?> TopRightNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(TopRightNotifications));

    public static readonly StyledProperty<IList?> BottomLeftNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(BottomLeftNotifications));

    public static readonly StyledProperty<IList?> BottomRightNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(BottomRightNotifications));

    public static readonly StyledProperty<IList?> CenterNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(CenterNotifications));

    public static readonly StyledProperty<IList?> LeftCenterNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(LeftCenterNotifications));

    public static readonly StyledProperty<IList?> RightCenterNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(RightCenterNotifications));

    public static readonly StyledProperty<IList?> TopCenterNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(TopCenterNotifications));

    public static readonly StyledProperty<IList?> BottomCenterNotificationsProperty =
        AvaloniaProperty.Register<NotificationPanel, IList?>(nameof(BottomCenterNotifications));

    public static readonly StyledProperty<string?> IdentifierProperty = AvaloniaProperty.Register<
        NotificationPanel,
        string?
    >(nameof(Identifier));

    static NotificationPanel()
    {
        IdentifierProperty.Changed.AddClassHandler<NotificationPanel, string?>(
            (control, id) =>
            {
                if (id.NewValue.Value is null)
                {
                    return;
                }

                NotificationPanels[id.NewValue.Value] = control;
            }
        );
    }

    public IList? TopLeftNotifications
    {
        get => GetValue(TopLeftNotificationsProperty);
        set => SetValue(TopLeftNotificationsProperty, value);
    }

    public IList? TopRightNotifications
    {
        get => GetValue(TopRightNotificationsProperty);
        set => SetValue(TopRightNotificationsProperty, value);
    }

    public IList? BottomLeftNotifications
    {
        get => GetValue(BottomLeftNotificationsProperty);
        set => SetValue(BottomLeftNotificationsProperty, value);
    }

    public IList? BottomRightNotifications
    {
        get => GetValue(BottomRightNotificationsProperty);
        set => SetValue(BottomRightNotificationsProperty, value);
    }

    public IList? CenterNotifications
    {
        get => GetValue(CenterNotificationsProperty);
        set => SetValue(CenterNotificationsProperty, value);
    }

    public IList? LeftCenterNotifications
    {
        get => GetValue(LeftCenterNotificationsProperty);
        set => SetValue(LeftCenterNotificationsProperty, value);
    }

    public IList? RightCenterNotifications
    {
        get => GetValue(RightCenterNotificationsProperty);
        set => SetValue(RightCenterNotificationsProperty, value);
    }

    public IList? TopCenterNotifications
    {
        get => GetValue(TopCenterNotificationsProperty);
        set => SetValue(TopCenterNotificationsProperty, value);
    }

    public IList? BottomCenterNotifications
    {
        get => GetValue(BottomCenterNotificationsProperty);
        set => SetValue(BottomCenterNotificationsProperty, value);
    }

    public string? Identifier
    {
        get => GetValue(IdentifierProperty);
        set => SetValue(IdentifierProperty, value);
    }

    public static void CloseNotification(string identifier, object notification)
    {
        if (!NotificationPanels.TryGetValue(identifier, out var panel))
        {
            throw new($"Notification panel {identifier} not found");
        }

        panel.TopLeftNotifications?.Remove(notification);
        panel.TopRightNotifications?.Remove(notification);
        panel.BottomLeftNotifications?.Remove(notification);
        panel.BottomRightNotifications?.Remove(notification);
        panel.CenterNotifications?.Remove(notification);
        panel.LeftCenterNotifications?.Remove(notification);
        panel.RightCenterNotifications?.Remove(notification);
        panel.TopCenterNotifications?.Remove(notification);
        panel.BottomCenterNotifications?.Remove(notification);
    }

    public static void ShowNotification(
        string identifier,
        object notification,
        NotificationPanelAlignment alignment,
        TimeSpan duration,
        bool isAddInEnd = true
    )
    {
        if (!NotificationPanels.TryGetValue(identifier, out var panel))
        {
            throw new($"Notification panel {identifier} not found");
        }

        var list = panel.GetOrCreateList(alignment);

        if (isAddInEnd)
        {
            list.Add(notification);
        }
        else
        {
            list.Insert(0, notification);
        }

        if (Timeout.InfiniteTimeSpan == duration)
        {
            return;
        }

        if (TimeSpan.Zero >= duration)
        {
            return;
        }

        Task.Run(async () =>
        {
            await Task.Delay(duration);
            list.Remove(notification);
        });
    }

    private IList GetOrCreateList(NotificationPanelAlignment alignment)
    {
        return alignment switch
        {
            NotificationPanelAlignment.TopLeft => GetOrCreateList(TopLeftNotificationsProperty),
            NotificationPanelAlignment.TopRight => GetOrCreateList(TopRightNotificationsProperty),
            NotificationPanelAlignment.BottomLeft => GetOrCreateList(
                BottomLeftNotificationsProperty
            ),
            NotificationPanelAlignment.BottomRight => GetOrCreateList(
                BottomRightNotificationsProperty
            ),
            NotificationPanelAlignment.Center => GetOrCreateList(CenterNotificationsProperty),
            NotificationPanelAlignment.LeftCenter => GetOrCreateList(
                LeftCenterNotificationsProperty
            ),
            NotificationPanelAlignment.RightCenter => GetOrCreateList(
                RightCenterNotificationsProperty
            ),
            NotificationPanelAlignment.TopCenter => GetOrCreateList(TopCenterNotificationsProperty),
            NotificationPanelAlignment.BottomCenter => GetOrCreateList(
                BottomCenterNotificationsProperty
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null),
        };
    }

    private IList GetOrCreateList(StyledProperty<IList?> listProperty)
    {
        var list = GetValue(listProperty);

        if (list is not null)
        {
            return list;
        }

        list = new AvaloniaList<object>();
        SetValue(listProperty, list);

        return list;
    }
}
