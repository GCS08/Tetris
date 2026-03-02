using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Models
{
    public abstract class SoundManagerModel
    {
        protected IAudioManager? audioManager;
        protected IAudioPlayer? lineClearedPlayer;
        protected IAudioPlayer? shapeDownPlayer;
        public abstract Task InitializeAsync();
        public abstract void PlayLineCleared();
        public abstract void PlayMoveDown();
    }
}
