namespace Tetris.Models
{
    /// <summary>
    /// Base model for handling network connectivity state.
    /// Provides a property to track connectivity and an event that is raised
    /// whenever the connectivity status changes.
    /// </summary>
    public abstract class ConnectivityModel
    {
        #region Fields

        /// <summary>
        /// Backing field for the <see cref="IsConnected"/> property.
        /// </summary>
        private bool _isConnected;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the connectivity state changes.
        /// Subscribers are notified whenever <see cref="IsConnected"/> is updated.
        /// </summary>
        public EventHandler? ConnectivityChanged { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the device currently has internet access.
        /// When the value changes, the <see cref="ConnectivityChanged"/> event is raised.
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            protected set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectivityChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles platform-specific connectivity change events.
        /// Must be implemented by derived classes to update <see cref="IsConnected"/>.
        /// </summary>
        /// <param name="sender">The source of the connectivity change event.</param>
        /// <param name="e">Event arguments containing the updated network access state.</param>
        protected abstract void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e);

        #endregion
    }
}