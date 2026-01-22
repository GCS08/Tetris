using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using System.Timers;
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
        public long TimeLeftMs { get; protected set; }
        public string TimeLeftText => (TimeLeftMs / 1000).ToString() == 
            TechnicalConsts.ZeroSignString ? Strings.TimeUp : (TimeLeftMs / 1000).ToString();
        public ObservableCollection<User> UsersInGame { get; set; } = [];
        public ICommand JoinGameCommand => new Command(NavToWR);
        public EventHandler? OnPlayersChange;
        public EventHandler? OnGameFull;
        public EventHandler? OnAllReady;
        public EventHandler? OnTimeLeftChanged;
        public GameBoard? GameBoard;
        public GameBoard? OpGameBoard;
        protected int desiredIndex = 0;
        protected string currentMovingOpId = string.Empty;
        protected FbData fbd = new();
        protected System.Timers.Timer OpFallTimer = new(ConstData.OpShapeFallIntervalS * 1000);
        protected ModelsLogic.Queue<KeyValuePair<string, string>> movesQueue = new();
        protected bool IsMovesQueueSorting { get; set; } = false;
        protected IListenerRegistration? ilr;
        protected StartGameTimerSettings startGameTimerSettings = 
            new(ConstData.TotalGameTimeS * 1000, ConstData.GameTimeIntervalS * 1000);
        protected abstract void RegisterTimer();
        protected abstract void OnMessageReceived(long timeLeft);
        public abstract Task OnPlayerLeaveWR();
        public abstract void AddWaitingRoomListener();
        public abstract void RemoveWaitingRoomListener();
        public abstract void AddGameListener();
        public abstract void RemoveGameListener();
        public abstract void AddReadyListener();
        public abstract void RemoveReadyListener();
        protected abstract void OnChangeReady(IDocumentSnapshot snapshot, Exception error);
        protected abstract void OnChangeGame(IDocumentSnapshot snapshot, Exception error);
        protected abstract void ApplyOpMove(object? sender, ElapsedEventArgs e);
        protected abstract void OnChangeWaitingRoom(IDocumentSnapshot snapshot, Exception error);
        protected abstract void OnCompleteChange(ObservableCollection<User> users);
        public abstract void PrepareGame();
        public abstract void StartGame();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract void MoveDownShape();
        public abstract void RotateShape();
        public abstract void MoveRightOpShape();
        public abstract void MoveLeftOpShape();
        public abstract void MoveDownOpShape();
        public abstract void RotateOpShape();
        public abstract void Ready();
        public abstract void NavToWR();
    }
}
