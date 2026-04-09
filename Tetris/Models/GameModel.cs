using System.Collections.ObjectModel;
using System.Windows.Input;
using Plugin.CloudFirestore;
using Tetris.Interfaces;
using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Abstract base class representing the core logic and state for a multiplayer game session, including player
    /// management, game timing, event handling, and game board operations.
    /// </summary>
    public abstract class GameModel
    {
        #region Fields
        protected User User = IPlatformApplication.
            Current?.Services.GetService<IUser>() as User ?? new();
        protected FbData fbd = IPlatformApplication.
            Current?.Services.GetService<IFbData>() as FbData ?? new();
        protected IDispatcherTimer? OpFallTimer;
        protected IListenerRegistration? ilr;
        protected StartGameTimerSettings startGameTimerSettings = 
            new(ConstData.TotalGameTimeS * 1000, ConstData.GameTimeIntervalS * 1000);
        #endregion
        
        #region Events
        public EventHandler? OnPlayersChange;
        public EventHandler? OnGameFull;
        public EventHandler? OnAllReady;
        public EventHandler? OnTimeLeftChanged;
        public EventHandler? OnGameFinishedUI;
        public EventHandler? OnCodeReady;
        #endregion
        
        #region ICommands
        public ICommand JoinGameCommand => new Command(NavToWR);
        #endregion
        
        #region Properties
        protected bool IsGameStarted { get; set; } = false;
        protected bool IsStatsUpdatedOnGameFinished { get; set; } = false;
        protected ModelsLogic.Queue<ModelsLogic.Queue<string>> OpMovesQueueOfQueues { get; set; } = new();
        public string CubeColor { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public int CurrentPlayersCount { get; set; }
        public int MaxPlayersCount { get; set; }
        public int PrivateJoinCode { get; set; }
        public bool IsFull => CurrentPlayersCount == MaxPlayersCount;
        public string UsersInGameSum => string.Empty + MaxPlayersCount
            + TechnicalConsts.SpaceSign + TechnicalConsts.SlashSign
            + TechnicalConsts.SpaceSign + CurrentPlayersCount;
        public bool IsPublicGame { get; set; }
        public string GameID { get; set; } = string.Empty;
        public long TimeLeftMs { get; protected set; }
        public string TimeLeftText => (TimeLeftMs / 1000).ToString() == TechnicalConsts.
            ZeroSignString ? Strings.TimeUp : (TimeLeftMs / 1000).ToString();
        public ObservableCollection<User> UsersInGame { get; set; } = [];
        public GameBoard? GameBoard { get; set; }
        public GameBoard? OpGameBoard { get; set; }
        #endregion
        
        #region Public Methods
        public abstract void RegisterTimer();
        public abstract Task OnPlayerLeaveWR();
        public abstract void AddWaitingRoomListener();
        public abstract void RemoveWaitingRoomListener();
        public abstract void AddGameListener();
        public abstract void RemoveGameListener();
        public abstract void AddReadyListener();
        public abstract void RemoveReadyListener();
        public abstract void PrepareGame();
        public abstract void StartGame();
        public abstract void MoveRightShape();
        public abstract void MoveLeftShape();
        public abstract void MoveDownShape();
        public abstract void SnapDownShape();
        public abstract void RotateShape();
        public abstract void Ready();
        public abstract void NavToWR();
        public abstract void CreateCode();
        public abstract void UpdateInternet(bool isConnected);
        public abstract Task<Game> GetGameByCode(int code);
        #endregion
       
        #region Protected Methods
        protected abstract void UnregisterTimer();
        protected abstract void OnGameFinishedLogicHandler(object? sender, EventArgs e);
        protected abstract void OnMessageReceived(long timeLeft);
        protected abstract void OnChangeReady(IDocumentSnapshot snapshot, Exception error);
        protected abstract void OnChangeGame(IDocumentSnapshot snapshot, Exception error);
        protected abstract void ProcessShapeChange(IDocumentSnapshot snapshot);
        protected abstract void ProcessMoveChange(IDocumentSnapshot snapshot);
        protected abstract void ApplyOpMove(object? sender, EventArgs e);
        protected abstract void OnChangeWaitingRoom(IDocumentSnapshot snapshot, Exception error);
        protected abstract void OnCompleteChange(ObservableCollection<User> users);
        protected abstract void MoveRightOpShape();
        protected abstract void MoveLeftOpShape();
        protected abstract void MoveDownOpShape();
        protected abstract void SnapDownOpShape();
        protected abstract void RotateOpShape();
        #endregion
    }
}
