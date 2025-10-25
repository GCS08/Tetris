using Tetris.ModelsLogic;

namespace Tetris.Models
{
    abstract class JoinableGamesListModel
    {
        protected readonly FbData fbd = new();
        public abstract Task<List<JoinableGame>> GetJoinableGamesAsync();
    }
}
