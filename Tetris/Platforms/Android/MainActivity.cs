using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        StartGameTimer? startGameTimer;
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RegisterTimerMessages();
            StartDeleteFbDocsService();
            _ = SoundManager.Instance.InitializeAsync();
            PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>().ContinueWith(OnComplete);
        }
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            if (intent != null)
                CreateNotificationFromIntent(intent);
        }
        static void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(NotificationManagerService.TitleKey) ?? string.Empty;
                string message = intent.GetStringExtra(NotificationManagerService.MessageKey) ?? string.Empty;
                INotificationManagerService? service = null;
                if (IPlatformApplication.Current != null)
                    service = IPlatformApplication.Current.Services.GetService<INotificationManagerService>();
                service?.ReceiveNotification(title, message);
            }
        }
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
        private void OnMessageReceived(bool value)
        {
            if (value)
            {
                startGameTimer?.Cancel();
                startGameTimer = null;
            }
        }
        private void OnMessageReceived(StartGameTimerSettings value)
        {
            startGameTimer = new StartGameTimer(value.TotalTimeInMilliseconds, value.IntervalInMilliseconds);
            startGameTimer.Start();
        }
        private void StartDeleteFbDocsService()
        {
            Intent intent = new(this, typeof(DeleteFbDocsService));
            StartService(intent);
        }
        private PermissionStatus OnComplete(Task<PermissionStatus> task)
        {
            return task.Result;
        }
    }
}
