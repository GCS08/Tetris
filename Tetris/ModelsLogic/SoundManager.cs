using Plugin.Maui.Audio;
using Tetris.Interfaces;
using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class SoundManager : SoundManagerModel, ISoundManager
    {
        #region Constructors
        public SoundManager()
        {
            audioManager = AudioManager.Current;
        }
        #endregion

        #region Public Methods
        public override async Task InitializeAsync()
        {
            if (audioManager == null) return;
            lineClearedPlayer = audioManager.CreatePlayer(
                    await FileSystem.OpenAppPackageFileAsync(TechnicalConsts.lineClearedPath));
            shapeDownPlayer = audioManager.CreatePlayer(
                    await FileSystem.OpenAppPackageFileAsync(TechnicalConsts.shapeDownPath));
        }

        public override void PlayLineCleared()
        {
            lineClearedPlayer?.Play();
        }

        public override void PlayMoveDown()
        {
            shapeDownPlayer?.Play();
        }
        #endregion
    }
}
