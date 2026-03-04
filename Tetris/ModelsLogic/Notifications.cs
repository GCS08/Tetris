using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Handles sending and scheduling notifications for the app, 
    /// using the platform-specific notification manager service.
    /// </summary>
    public class Notifications : NotificationsModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifications"/> class.
        /// Checks notification permissions and subscribes to the notification manager's events.
        /// </summary>
        public Notifications()
        {
            // Check notification permission asynchronously and store the result
            Permissions.CheckStatusAsync<Permissions.PostNotifications>()
                .ContinueWith(SetPermissionStatus);

            // Retrieve the platform-specific notification manager service from MAUI context
            notificationManager = Application.Current?.MainPage?.Handler?
                .MauiContext?.Services.GetService<INotificationManagerService>();

            // Subscribe to notification events if service is available
            if (notificationManager != null)
                notificationManager.NotificationReceived += OnNotificationReceived;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends a notification immediately or at a specified time if provided.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message content of the notification.</param>
        /// <param name="notifyTime">
        /// Optional. The time at which to display the notification. 
        /// If <c>null</c>, the notification is sent immediately.
        /// </param>
        /// <returns>
        /// <c>true</c> if the notification was successfully sent; <c>false</c> if permissions are not granted or the notification manager is unavailable.
        /// </returns>
        public override bool PushNotification(string title, string message, DateTime? notifyTime = null)
        {
            bool sent = false;
            // Check permissions again in case status changed
            Permissions.CheckStatusAsync<Permissions.PostNotifications>()
                .ContinueWith(SetPermissionStatus);

            if (notificationManager != null && PermissionStatus == PermissionStatus.Granted)
            {
                notificationManager.SendNotification(title, message, notifyTime);
                sent = true;
            }

            return sent;
        }

        /// <summary>
        /// Schedules a reminder notification at a specific date and time with optional seconds.
        /// </summary>
        /// <param name="selectedDate">The date for the reminder.</param>
        /// <param name="selectedTime">The time (hours and minutes) for the reminder.</param>
        /// <param name="selectedSeconds">The seconds portion as a string. Non-numeric values are treated as 0.</param>
        /// <returns>
        /// <c>true</c> if the reminder was successfully scheduled in the future; <c>false</c> if the calculated time is in the past.
        /// </returns>
        public override bool ScheduleReminder(DateTime selectedDate, TimeSpan selectedTime, string selectedSeconds)
        {
            bool result = true;
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

            // Do not schedule reminders in the past
            if (finalTime <= DateTime.Now)
                result = false;
            else
                PushNotification(
                    Strings.NotificationTitle,
                    Strings.NotificationContent,
                    finalTime
                );

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Callback used to set the permission status after an asynchronous permission check.
        /// </summary>
        /// <param name="task">The task representing the asynchronous permission check.</param>
        protected override void SetPermissionStatus(Task<PermissionStatus> task)
        {
            if (task.IsCompletedSuccessfully)
                PermissionStatus = task.Result;
        }

        /// <summary>
        /// Handler invoked when a notification is received from the notification manager service.
        /// Raises the NotificationReceived event for subscribers.
        /// </summary>
        /// <param name="sender">The object that raised the event, typically the notification manager service.</param>
        /// <param name="e">Event arguments containing the notification data. May be cast to <see cref="NotificationEventArgs"/>.</param>
        protected override void OnNotificationReceived(object? sender, EventArgs e)
        {
            NotificationReceived?.Invoke(this, (NotificationEventArgs)e);
        }

        #endregion
    }
}