namespace Tetris.Models
{
    /// <summary>
    /// Provides data for notification events, including a title and message.
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        #region Properties
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        #endregion
    }
}
