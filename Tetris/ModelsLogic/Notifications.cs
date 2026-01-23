using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Notifications : NotificationsModel
    {
        public Notifications()
        {
            notificationManager = Application.Current?.MainPage?.Handler?.MauiContext?.Services.GetService<INotificationManagerService>();
            if (notificationManager != null)
                notificationManager.NotificationReceived += OnNotificationReceived;
        }
        private void OnNotificationReceived(object? sender, EventArgs e)
        {
            NotificationArgs = (NotificationEventArgs)e;
            NotificationReceived?.Invoke(this, NotificationArgs);
        }
        public bool PushNotification(string title, string message, DateTime? notifyTime = null)
        {
            bool sent = false;
            if (notificationManager != null && PermissionStatus == PermissionStatus.Granted)
            {
                notificationManager.SendNotification(title, message, notifyTime);
                sent = true;
            }
            return sent;
        }
    }
}
