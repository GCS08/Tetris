using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Notifications : NotificationsModel
    {
        public Notifications()
        {
            Permissions.CheckStatusAsync<Permissions.PostNotifications>().ContinueWith(SetPermissionStatus);
            // Get the notification manager service from the MAUI context.
            // Obviously the app doesn't send notification, the maui does on command from the app, called from notification manager service.
            notificationManager = Application.Current?.MainPage?.Handler?.MauiContext?.Services.GetService<INotificationManagerService>();
            if (notificationManager != null)
                notificationManager.NotificationReceived += OnNotificationReceived;
        }
        protected override void SetPermissionStatus(Task<PermissionStatus> task)
        {
            if (task.IsCompletedSuccessfully)
                PermissionStatus = task.Result;
        }
        protected override void OnNotificationReceived(object? sender, EventArgs e)
        {
            NotificationReceived?.Invoke(this, (NotificationEventArgs)e);
        }
        public override bool PushNotification(string title, string message, DateTime? notifyTime = null)
        {
            bool sent = false;
            Permissions.CheckStatusAsync<Permissions.PostNotifications>().ContinueWith(SetPermissionStatus);
            if (notificationManager != null && PermissionStatus == PermissionStatus.Granted)
            {
                notificationManager.SendNotification(title, message, notifyTime);
                sent = true;
            }
            return sent;
        }

        public override bool ScheduleReminder(DateTime selectedDate, TimeSpan selectedTime, string selectedSeconds)
        {
            if (!int.TryParse(selectedSeconds, out int seconds) || seconds < 0 || seconds > 59)
                seconds = 0;

            DateTime finalTime = new(
                selectedDate.Year,
                selectedDate.Month,
                selectedDate.Day,
                selectedTime.Hours,
                selectedTime.Minutes,
                seconds
            );

            if (finalTime <= DateTime.Now)
                return false;

            PushNotification(
                Strings.NotificationTitle,
                Strings.NotificationContent,
                finalTime
            );
            return true;
        }
    }
}
