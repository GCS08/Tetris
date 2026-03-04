using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Tetris.Interfaces;
using Tetris.Models;
using Tetris.ModelsLogic;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// Main Android activity for the Tetris app. Handles initialization of services,
    /// timers, notifications, and sound manager on app launch.
    /// </summary>
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
                             | ConfigChanges.Orientation
                             | ConfigChanges.UiMode
                             | ConfigChanges.ScreenLayout
                             | ConfigChanges.SmallestScreenSize
                             | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        #region Fields

        /// <summary>
        /// Timer used to start games with countdowns.
        /// </summary>
        private StartGameTimer? startGameTimer;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when the activity is created. Initializes messaging, services, notifications, and sound.
        /// </summary>
        /// <param name="savedInstanceState">Bundle containing the activity's previously saved state, if any.</param>
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Register listeners for start game timer messages
            RegisterTimerMessages();

            // Start the background service for deleting Firebase documents
            StartDeleteFbDocsService();

            // Request notification permissions from the user
            _ = Permissions.RequestAsync<NotificationPermission>();

            // Initialize the sound manager if available
            if (IPlatformApplication.Current?.Services.GetService<ISoundManager>() is SoundManager soundManager)
                await soundManager.InitializeAsync();
        }

        /// <summary>
        /// Called when the activity receives a new intent while running in single-top mode.
        /// Handles notifications sent from intents.
        /// </summary>
        /// <param name="intent">The new intent received by the activity.</param>
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent != null)
                ReceiveNotificationFromIntent(intent);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Extracts notification data from an intent and passes it to the notification manager service.
        /// </summary>
        /// <param name="intent">Intent containing the notification extras.</param>
        private static void ReceiveNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(Keys.TitleKey) ?? string.Empty;
                string message = intent.GetStringExtra(Keys.MessageKey) ?? string.Empty;

                INotificationManagerService? service = null;
                if (IPlatformApplication.Current != null)
                    service = IPlatformApplication.Current.Services.GetService<INotificationManagerService>();

                service?.ReceiveNotification(title, message);
            }
        }

        /// <summary>
        /// Registers message listeners via WeakReferenceMessenger for handling start game timer events.
        /// </summary>
        private void RegisterTimerMessages()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<StartGameTimerSettings>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });

            WeakReferenceMessenger.Default.Register<AppMessage<bool>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        /// <summary>
        /// Starts a countdown timer based on the provided settings.
        /// </summary>
        /// <param name="value">Settings for the start game timer including total and interval times.</param>
        private void OnMessageReceived(StartGameTimerSettings value)
        {
            startGameTimer = new StartGameTimer(value.TotalTimeInMilliseconds, value.IntervalInMilliseconds);
            startGameTimer.Start();
        }

        /// <summary>
        /// Cancels the active start game timer if instructed to stop.
        /// </summary>
        /// <param name="value">Boolean indicating whether to cancel the timer.</param>
        private void OnMessageReceived(bool value)
        {
            if (value)
            {
                startGameTimer?.Cancel();
                startGameTimer = null;
            }
        }

        /// <summary>
        /// Starts the <see cref="DeleteFbDocsService"/> as a foreground service (Android O+) 
        /// or background service (pre-Oreo).
        /// </summary>
        private void StartDeleteFbDocsService()
        {
            Intent intent = new(this, typeof(DeleteFbDocsService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartForegroundService(intent);
            else
                StartService(intent);
        }

        #endregion
    }
}