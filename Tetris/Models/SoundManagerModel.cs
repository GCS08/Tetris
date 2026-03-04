using Plugin.Maui.Audio;

namespace Tetris.Models
{
    /// <summary>
    /// Abstract base class for managing sound playback in an application, providing methods for initialization and
    /// playing specific sound effects.
    /// </summary>
    public abstract class SoundManagerModel
    {
        #region Fields
        protected IAudioManager? audioManager;
        protected IAudioPlayer? lineClearedPlayer;
        protected IAudioPlayer? shapeDownPlayer;
        #endregion

        #region Public Methods
        public abstract Task InitializeAsync();
        public abstract void PlayLineCleared();
        public abstract void PlayMoveDown();
        #endregion
    }
}
