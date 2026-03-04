using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// Service responsible for managing notifications on Android.
    /// Handles immediate notifications, scheduled notifications via AlarmManager,
    /// and raises <see cref="NotificationReceived"/> events for app consumption.
    /// </summary>
    public class NotificationManagerService : NotificationManagerServiceModel
    {
        #region Events

        /// <summary>
        /// Event triggered when a notification is received, either via scheduled alarm or immediate push.
        /// </summary>
        public override event EventHandler? NotificationReceived;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="NotificationManagerService"/>.
        /// Ensures singleton Instance is set and notification channel is created.
        /// </summary>
        public NotificationManagerService()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                notificationManager = NotificationManagerCompat.From(Platform.AppContext);
                Instance = this;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds an Android notification with the specified title and message.
        /// </summary>
        /// <param name="title">Title text of the notification.</param>
        /// <param name="message">Message/body text of the notification.</param>
        /// <returns>A fully built <see cref="Notification"/> object.</returns>
        public override Notification BuildNotification(string title, string message)
        {
            Intent intent = new(Platform.AppContext, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            PendingIntentFlags pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent;

            PendingIntent? pendingIntent = PendingIntent.GetActivity(
                Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);

            NotificationCompat.Builder? builder = new NotificationCompat.Builder(Platform.AppContext, Keys.NotificationChannelId)
                .SetContentIntent(pendingIntent)!
                .SetContentTitle(title)!
                .SetContentText(message)!
                .SetSmallIcon(Resource.Drawable.appiconsmall)!
                .SetLargeIcon(BitmapFactory.DecodeResource(Platform.AppContext.Resources, Resource.Drawable.appiconbig))!
                .SetColor(Colors.Blue.ToInt());

            return builder?.Build() ?? null!;
        }

        /// <summary>
        /// Immediately displays a notification with the given title and message.
        /// </summary>
        /// <param name="title">Title text of the notification.</param>
        /// <param name="message">Message/body text of the notification.</param>
        public override void Show(string title, string message)
        {
            notificationManager?.Notify(messageId++, BuildNotification(title, message));
        }

        /// <summary>
        /// Sends a notification immediately or schedules it for a future time using AlarmManager.
        /// </summary>
        /// <param name="title">Title text of the notification.</param>
        /// <param name="message">Message/body text of the notification.</param>
        /// <param name="notifyTime">Optional time for the notification to appear. If null, shows immediately.</param>
        public override void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (!channelInitialized)
                CreateNotificationChannel();

            if (notifyTime != null)
            {
                Intent intent = new(Platform.AppContext, typeof(AlarmReceiver));
                intent.PutExtra(Keys.TitleKey, title);
                intent.PutExtra(Keys.MessageKey, message);
                intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

                PendingIntentFlags pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                    ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                    : PendingIntentFlags.CancelCurrent;

                PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
                    Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);

                long triggerTime = GetNotifyTime(notifyTime.Value);
                AlarmManager? alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
                if (pendingIntent != null)
                    alarmManager?.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
            else
                Show(title, message);
        }

        /// <summary>
        /// Raises the <see cref="NotificationReceived"/> event with the given title and message.
        /// </summary>
        /// <param name="title">Notification title.</param>
        /// <param name="message">Notification message.</param>
        public override void ReceiveNotification(string title, string message)
        {
            NotificationEventArgs args = new()
            {
                Title = title,
                Message = message,
            };
            Instance?.NotificationReceived?.Invoke(null, args);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the Android notification channel (required on API 26+).
        /// </summary>
        protected override void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Java.Lang.String channelNameJava = new(Keys.NotificationChannelName);
                NotificationChannel channel = new(Keys.NotificationChannelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = Strings.NotificationChannelDescription
                };

                NotificationManager? manager = Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;
                manager?.CreateNotificationChannel(channel);
                channelInitialized = true;
            }
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> into milliseconds since Unix epoch for AlarmManager scheduling.
        /// </summary>
        /// <param name="notifyTime">The local <see cref="DateTime"/> to trigger the notification.</param>
        /// <returns>Time in milliseconds since Unix epoch.</returns>
        protected override long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }

        #endregion
    }
}