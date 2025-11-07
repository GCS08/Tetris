using System.Windows.Input;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameModel(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, string GameID)
    {
        public string CubeColor { get; set; } = CubeColor;
        public string CreatorName { get; set; } = CreatorName;
        public int CurrentPlayersCount { get; set; } = CurrentPlayersCount;
        public int MaxPlayersCount { get; set; } = MaxPlayersCount;
        public string UsersInGameSum => $"{MaxPlayersCount} / {CurrentPlayersCount}";
        public bool IsPublicGame { get; set; } = IsPublicGame;
        public string GameID { get; set; } = GameID;
        protected FbData fbd = new();
        public ICommand JoinGameCommand => new Command(NavToHome);
        private async void NavToHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
