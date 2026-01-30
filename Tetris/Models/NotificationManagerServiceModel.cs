using AndroidX.Core.App;
using Tetris.Platforms.Android;

namespace Tetris.Models
{
    public abstract class NotificationManagerServiceModel : INotificationManagerService
    {
        protected bool channelInitialized = false;
        protected int pendingIntentId = 0, messageId;
        protected NotificationManagerCompat? notificationManager;
        public static NotificationManagerService? Instance { get; set; }
        public abstract event EventHandler? NotificationReceived;
        public abstract void SendNotification(string title, string message, DateTime? notifyTime = null);
        public abstract void ReceiveNotification(string title, string message);
        public abstract void Show(string title, string message);
        protected abstract void CreateNotificationChannel();
        protected abstract long GetNotifyTime(DateTime notifyTime);
    }
}
