using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Monitors the device's network connectivity status.
    /// Updates the connection state whenever network access changes.
    /// </summary>
    public class Connectivity : ConnectivityModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Connectivity"/> class.
        /// Subscribes to connectivity change events and sets the initial connection state.
        /// </summary>
        public Connectivity()
        {
            Microsoft.Maui.Networking.Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
            IsConnected = Microsoft.Maui.Networking.Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles connectivity changes from the system.
        /// Updates the IsConnected property based on current network access.
        /// </summary>
        /// <param name="sender">The source of the connectivity change event.</param>
        /// <param name="e">Event arguments containing the updated network access state.</param>
        protected override void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.NetworkAccess == NetworkAccess.Internet;
        }

        #endregion
    }
}