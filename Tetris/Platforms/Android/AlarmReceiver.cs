using Android.Content;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// BroadcastReceiver for handling alarm events on Android.
    /// Triggers notifications at the scheduled time using the NotificationManagerService.
    /// </summary>
    [BroadcastReceiver(Enabled = true, Label = Strings.AlarmReceiverBroadcastLabel)]
    public class AlarmReceiver : BroadcastReceiver
    {
        #region Public Methods

        /// <summary>
        /// Called when the alarm broadcast is received.
        /// Extracts notification title and message from the intent extras and displays the notification.
        /// </summary>
        /// <param name="context">The Android context from which the broadcast is received.</param>
        /// <param name="intent">The intent containing notification data.</param>
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(Keys.TitleKey) ?? string.Empty;
                string message = intent.GetStringExtra(Keys.MessageKey) ?? string.Empty;

                // Get or create a singleton instance of NotificationManagerService
                NotificationManagerService manager = NotificationManagerService.Instance
                    ?? new NotificationManagerService();

                // Show the notification
                manager.Show(title, message);
            }
        }

        #endregion
    }
}