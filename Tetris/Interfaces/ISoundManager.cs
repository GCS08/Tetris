namespace Tetris.Interfaces
{
    public interface ISoundManager
    {
        #region Public Methods
        public Task InitializeAsync();
        public void PlayLineCleared();
        public void PlayMoveDown();
        #endregion
    }
}
