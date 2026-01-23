namespace Tetris.Models
{
    public class NotificationsModel
    {
        protected INotificationManagerService? notificationManager;
        public NotificationEventArgs? NotificationArgs { get; set; }
        public EventHandler<NotificationEventArgs>? NotificationReceived;
        public static PermissionStatus PermissionStatus => Permissions
            .CheckStatusAsync<Permissions.PostNotifications>().Result;
    }
}
