using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// Android foreground service for periodically deleting Firebase documents in the background.
    /// Runs continuously until stopped or the app is terminated.
    /// </summary>
    [Service(
        Name = "tetris.platforms.android.DeleteFbDocsService",
        Exported = true,
        ForegroundServiceType = ForegroundService.TypeDataSync)]
    public class DeleteFbDocsService : Service
    {
        #region Fields

        /// <summary>
        /// Flag to control the service loop.
        /// </summary>
        private bool isRunning = true;

        /// <summary>
        /// Firebase data service for deleting documents.
        /// </summary>
        private readonly FbData fbd = IPlatformApplication.Current?
            .Services.GetService<IFbData>() as FbData ?? new();

        /// <summary>
        /// Notification manager service to show foreground notifications.
        /// </summary>
        private readonly NotificationManagerService? notificationManager = IPlatformApplication
            .Current?.Services.GetService<INotificationManagerService>() as NotificationManagerService;

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the service is started. 
        /// Shows a foreground notification and starts the background deletion loop.
        /// </summary>
        /// <param name="intent">The intent that started the service. May be null.</param>
        /// <param name="flags">Flags indicating how the service was started.</param>
        /// <param name="startId">Unique integer representing this specific start request.</param>
        /// <returns>
        /// Returns <see cref="StartCommandResult.Sticky"/> so the system restarts the service if killed.
        /// </returns>
        public override StartCommandResult OnStartCommand(
            Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (notificationManager != null)
            {
                // Build and show a foreground notification to keep the service alive
                Notification notification = notificationManager.BuildNotification(
                    Strings.TetrisServiceTitle,
                    Strings.DeleteDocsDesc
                );
                StartForeground(1, notification);
            }

            // Run the deletion loop in a background task
            Task.Run(async () =>
            {
                while (isRunning)
                {
                    await fbd.DeleteFbDocsAsync(); // Delete documents from Firebase
                    await Task.Delay(ConstData.DeleteFbDocsIntervalS * 1000); // Wait interval
                }

                // Stop the service once loop ends
                StopSelf();
            });

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Called when the service is destroyed. Stops the deletion loop.
        /// </summary>
        public override void OnDestroy()
        {
            isRunning = false;
            base.OnDestroy();
        }

        /// <summary>
        /// This service does not support binding.
        /// </summary>
        /// <param name="intent">The intent that attempted to bind to the service.</param>
        /// <returns>Always returns null.</returns>
        public override IBinder? OnBind(Intent? intent) => null;

        #endregion
    }
}