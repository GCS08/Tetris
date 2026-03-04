using Tetris.Interfaces;

namespace Tetris.Models
{
    /// <summary>
    /// Provides an abstract base for managing notifications, including sending push notifications, scheduling
    /// reminders, and handling notification permissions.
    /// </summary>
    public abstract class NotificationsModel
    {
        #region Fields
        protected INotificationManagerService? notificationManager;
        #endregion

        #region Events
        public EventHandler<NotificationEventArgs>? NotificationReceived;
        #endregion
        
        #region Properties
        protected PermissionStatus PermissionStatus { get; set; }
            = Permissions.CheckStatusAsync<Permissions.PostNotifications>().Result;
        #endregion

        #region Public Methods
        public abstract bool PushNotification(string title, 
            string message, DateTime? notifyTime = null);
        public abstract bool ScheduleReminder(DateTime selectedDate,
            TimeSpan selectedTime, string selectedSeconds);
        #endregion

        #region Protected Methods
        protected abstract void SetPermissionStatus(Task<PermissionStatus> task);
        protected abstract void OnNotificationReceived(object? sender, EventArgs e);
        #endregion
    }
}
