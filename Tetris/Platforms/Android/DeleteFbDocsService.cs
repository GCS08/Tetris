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
    [Service(
    Name = "tetris.platforms.android.DeleteFbDocsService",
    Exported = true,
    ForegroundServiceType = ForegroundService.TypeDataSync)]
    public class DeleteFbDocsService : Service
    {
        private bool isRunning = true;
        private readonly FbData fbd = IPlatformApplication.Current?.
            Services.GetService<IFbData>() as FbData ?? new();
        private readonly NotificationManagerService? notificationManager = IPlatformApplication.
            Current?.Services.GetService<INotificationManagerService>() as NotificationManagerService;
        public override StartCommandResult OnStartCommand(
            Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (notificationManager != null)
            {
                Notification notification = notificationManager.BuildNotification(
                    Strings.TetrisServiceTitle,
                    Strings.DeleteDocsDesc
                );
                StartForeground(1, notification);
            }

            Task.Run(async () =>
            {
                while (isRunning)
                {
                    await fbd.DeleteFbDocsAsync();
                    await Task.Delay(ConstData.DeleteFbDocsIntervalS * 1000);
                }
                StopSelf();
            });

            return StartCommandResult.Sticky;
        }
      
        public override void OnDestroy()
        {
            isRunning = false;
            base.OnDestroy();
        }

        public override IBinder? OnBind(Intent? intent) => null;
    }
}