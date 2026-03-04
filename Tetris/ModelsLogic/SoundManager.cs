using Plugin.Maui.Audio;
using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Manages game sound effects for Tetris events such as line clears and shape drops.
    /// </summary>
    public class SoundManager : SoundManagerModel, ISoundManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="SoundManager"/> instance and retrieves
        /// the current MAUI audio manager.
        /// </summary>
        public SoundManager()
        {
            audioManager = AudioManager.Current;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asynchronously initializes the audio players for Tetris sound effects.
        /// Loads the "line cleared" and "shape down" audio files from the app package.
        /// </summary>
        public override async Task InitializeAsync()
        {
            if (audioManager == null) return;

            lineClearedPlayer = audioManager.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync(TechnicalConsts.lineClearedPath));

            shapeDownPlayer = audioManager.CreatePlayer(
                await FileSystem.OpenAppPackageFileAsync(TechnicalConsts.shapeDownPath));
        }

        /// <summary>
        /// Plays the sound effect for clearing a line.
        /// </summary>
        public override void PlayLineCleared()
        {
            lineClearedPlayer?.Play();
        }

        /// <summary>
        /// Plays the sound effect for a shape moving down.
        /// </summary>
        public override void PlayMoveDown()
        {
            shapeDownPlayer?.Play();
        }

        #endregion
    }
}