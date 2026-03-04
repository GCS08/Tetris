namespace Tetris.Interfaces
{
    public interface INotificationManagerService
    {
        #region Events
        event EventHandler NotificationReceived;
        #endregion

        #region Public Methods
        public void SendNotification(string title, string message, 
            DateTime? notifyTime = null);
        public void ReceiveNotification(string title, string message);
        #endregion
    }
}
