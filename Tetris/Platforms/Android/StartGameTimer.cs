using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    public class StartGameTimer(long millisInFuture, long countDownInterval) : CountDownTimer(millisInFuture, countDownInterval)
    {
        private int counter = 0;
        public override void OnFinish()
        {
            System.Diagnostics.Debug.WriteLine("Timer Finished " + counter);
            counter++;
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(ConstData.FinishedSignal));
        }
        public override void OnTick(long millisUntilFinished)
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(millisUntilFinished));
        }
    }
}
