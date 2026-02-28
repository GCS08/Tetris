using Tetris.Interfaces;

namespace Tetris.Models
{
    public abstract class NotificationsModel
    {
        protected INotificationManagerService? notificationManager;
        public EventHandler<NotificationEventArgs>? NotificationReceived;
        protected PermissionStatus PermissionStatus { get; set; }
            = Permissions.CheckStatusAsync<Permissions.PostNotifications>().Result;
        protected abstract void SetPermissionStatus(Task<PermissionStatus> task);
        protected abstract void OnNotificationReceived(object? sender, EventArgs e);
        public abstract bool PushNotification(string title, string message, DateTime? notifyTime = null);
        public abstract bool ScheduleReminder(DateTime selectedDate, TimeSpan selectedTime, string selectedSeconds);
    }
}
