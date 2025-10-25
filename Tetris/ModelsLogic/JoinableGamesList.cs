using Tetris.Models;

namespace Tetris.ModelsLogic
{
    internal class JoinableGamesList : JoinableGamesListModel
    {
        public override async Task<List<JoinableGame>> GetJoinableGamesAsync() => await fbd.GetJoinableGamesAsync();
    }
}
