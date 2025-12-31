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
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RegisterTimerMessages();
            StartDeleteFbDocsService();
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
    }
}
