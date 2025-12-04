using Inanna.Controls;
using Inanna.Helpers;
using Inanna.Models;

namespace Inanna.Services;

public interface INotificationService
{
    void ShowNotification(object content, NotificationType type);
}

public class NotificationService : INotificationService
{
    private readonly string _identifier;
    private readonly TimeSpan _duration;

    public NotificationService(string identifier, TimeSpan duration)
    {
        _identifier = identifier;
        _duration = duration;
    }

    public void ShowNotification(object content, NotificationType type)
    {
        var notification = new NotificationControl
        {
            Type = type,
            Content = content,
        };

        notification.Command = UiHelper.CreateCommand(_ =>
        {
            NotificationPanel.CloseNotification(_identifier, notification);

            return ValueTask.CompletedTask;
        });

        NotificationPanel.ShowNotification(_identifier, notification, NotificationPanelAlignment.Center, _duration);
    }
}