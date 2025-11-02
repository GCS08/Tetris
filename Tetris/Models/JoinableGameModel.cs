using System.Windows.Input;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class JoinableGameModel
    {
        public string CubeColor { get; set; }
        public string CreatorName { get; set; }
        public int CurrentPlayersCount { get; set; }
        public int MaxPlayersCount { get; set; }
        public string UsersInGameSum { get; set; }
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; }
        public ICommand JoinGameCommand { get; set; }
        protected FbData fbd = new();
        public abstract void NavToGame();
        public JoinableGameModel(string CubeColor, string CreatorName, int CurrentPlayersCount,
            int MaxPlayersCount, bool IsPublicGame, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            UsersInGameSum = $"{MaxPlayersCount} / {CurrentPlayersCount}";
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            JoinGameCommand = new Command(NavToGame);
        }
    }
}
