using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    public class StartGameTimer(long millisInFuture, long countDownInterval) 
        : CountDownTimer(millisInFuture, countDownInterval)
    {
        #region Public Methods
        public override void OnFinish()
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(ConstData.FinishedSignal));
        }
     
        public override void OnTick(long millisUntilFinished)
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(millisUntilFinished));
        }
        #endregion
    }
}
