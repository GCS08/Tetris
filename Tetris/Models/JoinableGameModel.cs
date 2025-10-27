using System.Windows.Input;

namespace Tetris.Models
{
    public abstract class JoinableGameModel
    {
        public string CubeColor { get; set; }
        public string CreatorName { get; set; }
        public int CurrentPlayersCount { get; set; }
        public int MaxPlayersCount { get; set; }
        public string GameID { get; set; }
        public ICommand JoinGameCommand { get; set; }
        public abstract void NavToGame();

        public JoinableGameModel(string CubeColor, string CreatorName, int CurrentPlayersCount,
            int MaxPlayersCount, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.GameID = GameID;
            JoinGameCommand = new Command(NavToGame);
        }
    }
}
