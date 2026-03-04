using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Tetris.Models;

namespace Tetris.Platforms.Android
{
    /// <summary>
    /// Represents a countdown timer used to manage the start of a Tetris game.
    /// Sends updates and a finished signal via <see cref="WeakReferenceMessenger"/>.
    /// </summary>
    /// <param name="millisInFuture">
    /// The total duration of the countdown in milliseconds.
    /// </param>
    /// <param name="countDownInterval">
    /// The interval in milliseconds at which <see cref="OnTick(long)"/> is called.
    /// </param>
    public class StartGameTimer(long millisInFuture, long countDownInterval)
        : CountDownTimer(millisInFuture, countDownInterval)
    {
        #region Public Methods

        /// <summary>
        /// Called when the countdown finishes.
        /// Sends a finished signal using <see cref="WeakReferenceMessenger"/>.
        /// </summary>
        public override void OnFinish()
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(ConstData.FinishedSignal));
        }

        /// <summary>
        /// Called at every tick of the countdown.
        /// Sends the remaining time in milliseconds using <see cref="WeakReferenceMessenger"/>.
        /// </summary>
        /// <param name="millisUntilFinished">
        /// The number of milliseconds remaining until the countdown finishes.
        /// </param>
        public override void OnTick(long millisUntilFinished)
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(millisUntilFinished));
        }

        #endregion
    }
}