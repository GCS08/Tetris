using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class GameModel
    {
        public string CubeColor { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public int CurrentPlayersCount { get; set; }
        public int MaxPlayersCount { get; set; }
        public bool IsFull => CurrentPlayersCount == MaxPlayersCount;
        public string UsersInGameSum => $"{MaxPlayersCount} / {CurrentPlayersCount}";
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; } = string.Empty;
        public ObservableCollection<User> UsersInGame { get; set; } = [];
        public ICommand JoinGameCommand => new Command(NavToWR);
        public EventHandler? OnPlayersChange;
        public EventHandler? OnGameFull;
        public EventHandler? OnAllReady;
        public GameBoard? GameBoard;
        public GameBoard? OpGameBoard;
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        public abstract void NavToWR();
    }
}
