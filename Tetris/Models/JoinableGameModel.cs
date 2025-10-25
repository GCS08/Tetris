using System.Windows.Input;

namespace Tetris.Models
{
    public abstract class JoinableGameModel
    {
        public string CubeColor { get; set; } = "Transparent";
        public string CreatorName { get; set; } = "Anonymous";
        public int CurrentPlayersCount { get; set; } = 0;
        public int MaxPlayersCount { get; set; } = 4;
        public string GameID { get; set; } = string.Empty;
        public ICommand? JoinGameCommand { get; set; }
        public abstract void NavToGame();
    }
}
