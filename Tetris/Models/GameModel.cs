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
        public Shape? CurrentShape { get; set; }
        public bool IsFull => CurrentPlayersCount == MaxPlayersCount;
        public string UsersInGameSum => $"{MaxPlayersCount} / {CurrentPlayersCount}";
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; } = string.Empty;
        public ModelsLogic.Queue<Shape>? ShapesQueue { get; set; } = new();
        public ObservableCollection<User> UsersInGame { get; set; } = [];
        protected FbData fbd = new();
        public ICommand JoinGameCommand => new Command(NavToWR);
        protected IListenerRegistration? ilr;
        public EventHandler? OnPlayersChange;
        public EventHandler? OnGameFull;
        public abstract void NavToWR();
    }
}
