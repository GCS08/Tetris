using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class JoinableGame : JoinableGameModel
    {
        public JoinableGame(string CubeColor,string CreatorName, int CurrentPlayersCount,
            int MaxPlayersCount, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.GameID = GameID;
        }
    }
}
