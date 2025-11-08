using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameModel
    {
        public string CubeColor { get; set; }
        public string CreatorName { get; set; }
        public int CurrentPlayersCount { get; set; }
        public int MaxPlayersCount { get; set; }
        public string UsersInGameSum => $"{MaxPlayersCount} / {CurrentPlayersCount}";
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; }
        public ObservableCollection<User> UsersInGame { get; set; }
        protected FbData fbd = new();
        public ICommand JoinGameCommand => new Command(NavToHome);
        protected IListenerRegistration? ilr;
        public EventHandler? OnPlayersChange;
        public GameModel(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.UsersInGame = [];
            UsersInGame.Add((Application.Current as App)!.user);
        }
        private async void NavToHome()
        {
            await Shell.Current.GoToAsync(TechnicalConsts.RedirectMainPageRefresh);
        }
    }
}
