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
        public string UsersInGameSum => string.Empty + MaxPlayersCount
            + TechnicalConsts.SpaceSign + TechnicalConsts.SlashSign
            + TechnicalConsts.SpaceSign + CurrentPlayersCount;
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; } = string.Empty;
        public ObservableCollection<User> UsersInGame { get; set; } = [];
        public ICommand JoinGameCommand => new Command(NavToWR);
        public EventHandler? OnPlayersChange;
        public EventHandler? OnGameFull;
        public EventHandler? OnAllReady;
        public GameBoard? GameBoard;
        public GameBoard? OpGameBoard;
        protected int desiredIndex = 0;
        protected string currentMovingOp = string.Empty;
        protected FbData fbd = new();
        protected System.Timers.Timer OpFallTimer = new(ConstData.OpShapeFallIntervalS * 1000);
        protected ModelsLogic.Queue<string> movesQueue = new();
        protected IListenerRegistration? ilr;
        public abstract void NavToWR();
    }
}
