using Tetris.Models;

namespace Tetris.ModelsLogic
{
    internal class JoinableGamesList : JoinableGamesListModel
    {
        public JoinableGamesList() { games = fbd.GetJoinableGamesAsync().Result; }
    }
}
