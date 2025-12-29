using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    public class DeleteFbDocsService : Service
    {
        private bool isRunning = true;
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            ThreadStart threadStart = new(DeleteFbDocs);
            Thread thread = new(threadStart);
            thread.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        private void DeleteFbDocs()
        {
            while (isRunning)
            {
                // Here will be the code to delete old documents from Firebase Firestore
                Thread.Sleep(ConstData.DeleteFbDocsIntervalS * 1000); // 1 hour
            }
            StopSelf();
        }
        public override void OnDestroy()
        {
            isRunning = false;
            base.OnDestroy();
        }
        public override IBinder? OnBind(Intent? intent)
        {
            // We don't use it
            return null;
        }
    }
}
