using Gaia.Helpers;
using Inanna.Controls;
using Inanna.Models;

namespace Inanna.Services;

public interface INotificationService
{
    void ShowNotification(object content, NotificationType type);
}

public sealed class NotificationService : INotificationService
{
    public NotificationService(string identifier, TimeSpan duration, ICommandFactory commandFactory)
    {
        _identifier = identifier;
        _duration = duration;
        _commandFactory = commandFactory;
    }

    public void ShowNotification(object content, NotificationType type)
    {
        var notification = new NotificationControl { Type = type, Content = content };

        notification.Command = _commandFactory.CreateCommand(_ =>
        {
            NotificationPanel.CloseNotification(_identifier, notification);

            return TaskHelper.ConfiguredCompletedTask;
        });

        NotificationPanel.ShowNotification(
            _identifier,
            notification,
            NotificationPanelAlignment.Center,
            _duration
        );
    }

    private readonly string _identifier;
    private readonly TimeSpan _duration;
    private readonly ICommandFactory _commandFactory;
}
