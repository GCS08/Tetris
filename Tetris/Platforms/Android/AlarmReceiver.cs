using Android.Content;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Label = Strings.AlarmReceiverBroadcastLabel)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent?.GetStringExtra(Keys.TitleKey) ?? string.Empty;
                string message = intent?.GetStringExtra(Keys.MessageKey) ?? string.Empty;
                NotificationManagerService manager = NotificationManagerService.Instance ?? new NotificationManagerService();
                manager.Show(title, message);
            }
        }
    }
}
