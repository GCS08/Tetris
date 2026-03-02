namespace Tetris.Interfaces
{
    public interface ISoundManager
    {
        Task InitializeAsync();
        void PlayLineCleared();
        void PlayMoveDown();
    }
}
