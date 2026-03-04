using Android.App;
using AndroidX.Core.App;
using Tetris.Interfaces;
using Tetris.Platforms.Android;

namespace Tetris.Models
{
    /// <summary>
    /// Provides an abstract base for managing notifications, including sending, receiving, and building notifications,
    /// as well as handling notification channels.
    /// </summary>
    public abstract class NotificationManagerServiceModel : INotificationManagerService
    {
        #region Fields
        protected bool channelInitialized = false;
        protected int pendingIntentId = 0, messageId;
        protected NotificationManagerCompat? notificationManager;
        #endregion

        #region Events
        public abstract event EventHandler? NotificationReceived;
        #endregion

        #region Poperties
        public static NotificationManagerService? Instance { get; set; }
        #endregion

        #region Public Methods
        public abstract void SendNotification(string title, 
            string message, DateTime? notifyTime = null);
        public abstract void ReceiveNotification(string title, string message);
        public abstract Notification BuildNotification(string title, string message);
        public abstract void Show(string title, string message);
        #endregion

        #region Protected Methods
        protected abstract void CreateNotificationChannel();
        protected abstract long GetNotifyTime(DateTime notifyTime);
        #endregion
    }
}
