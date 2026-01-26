using Plugin.Maui.Audio;

namespace Tetris.Platforms.Android
{
    public class SoundManager
    {
        private static SoundManager? _instance;
        public static SoundManager Instance =>
            _instance ??= new SoundManager(AudioManager.Current);

        private readonly IAudioManager audioManager;
        private IAudioPlayer? lineClearedPlayer;

        private SoundManager(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        public async Task InitializeAsync()
        {
            lineClearedPlayer =
                audioManager.CreatePlayer(
                    await FileSystem.OpenAppPackageFileAsync("lineCleared.mp3"));
        }

        public void PlayLineCleared()
        {
            lineClearedPlayer?.Play();
        }
    }
}
