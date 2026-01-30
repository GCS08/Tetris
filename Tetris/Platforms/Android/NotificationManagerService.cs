using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.Platforms.Android
{
    public class NotificationManagerService : NotificationManagerServiceModel
    {
        public override event EventHandler? NotificationReceived;
        public NotificationManagerService()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                notificationManager = NotificationManagerCompat.From(Platform.AppContext);
                Instance = this;
            }
        }
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

                PendingIntent? pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
                long triggerTime = GetNotifyTime(notifyTime.Value);
                AlarmManager? alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
                if (pendingIntent != null)
                    alarmManager?.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
            else
                Show(title, message);
        }
        public override void ReceiveNotification(string title, string message)
        {
            NotificationEventArgs args = new()
            {
                Title = title,
                Message = message,
            };
            // Raise the event using the protected member, which is allowed
            Instance?.NotificationReceived?.Invoke(null, args);
        }
        public override void Show(string title, string message)
        {
            Intent intent = new(Platform.AppContext, typeof(MainActivity));
            intent.PutExtra(Keys.TitleKey, title);
            intent.PutExtra(Keys.MessageKey, message);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            PendingIntentFlags pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent;
            PendingIntent? pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, Keys.NotificationChannelId)
                .SetContentIntent(pendingIntent)
                .SetColor(Colors.Blue.ToInt())
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Platform.AppContext.Resources, Resource.Drawable.appiconbig))
                .SetSmallIcon(Resource.Drawable.appiconsmall);

            Notification notification = builder.Build();
            notificationManager?.Notify(messageId++, notification);
        }
        protected override void CreateNotificationChannel()
        {
            // Create the notification channel, but only on API 26+.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Java.Lang.String channelNameJava = new(Keys.NotificationChannelName);
                NotificationChannel channel = new(Keys.NotificationChannelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = Strings.NotificationChannelDescription
                };
                // Register the channel
                NotificationManager? manager = Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;
                manager?.CreateNotificationChannel(channel);
                channelInitialized = true;
            }
        }
        protected override long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }
    }
}
