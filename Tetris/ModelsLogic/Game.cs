using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    public class Game : GameModel
    {
        #region Constructors
        public Game() { }
        public Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, Shape shape, string GameID)
        {
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.GameBoard = new(GameID, shape, false);
            this.OpGameBoard = new(GameID, shape.Duplicate(shape), true);
            OpGameBoard.OnGameFinishedLogic += OnGameFinishedLogicHandler;
            GameBoard.OnGameFinishedLogic += OnGameFinishedLogicHandler;
        }
        #endregion

        #region Public Methods
        public override async Task<Game> GetGameByCode(int code)
        {
            Game currentGame = await fbd.GetGameByCode(code);
            if (currentGame != null)
                return currentGame;
            return null!;
        }

        public override void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }
    
        public override async Task OnPlayerLeaveWR() 
        {
            if (CurrentPlayersCount <= 1)
            {
                ilr?.Remove();
                fbd.DeleteGameFromDB(GameID);
            }
            else
            {
                await fbd.OnPlayerLeaveWR(GameID, User.UserID);
                CurrentPlayersCount--;
                UsersInGame.Remove(User);
            }            
        }
    
        public override void AddWaitingRoomListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeWaitingRoom);
        }
    
        public override void RemoveWaitingRoomListener()
        {
            ilr?.Remove();
        }
  
        public override void AddGameListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeGame);
        }
   
        public override void RemoveGameListener()
        {
            ilr?.Remove();
        }
 
        public override void AddReadyListener()
        {
            ilr = fbd.AddGameListener(GameID, OnChangeReady);
        }
    
        public override void RemoveReadyListener()
        {
            ilr?.Remove();
        }
     
        public override async void NavToWR()
        {
            if (User == null) return;

            await fbd.OnPlayerJoinWR(GameID, User.UserID);
            CurrentPlayersCount++;

            _ = Shell.Current.Navigation.PushAsync(new WaitingRoomPage(this));
        }
      
        public override void PrepareGame()
        {
            WeakReferenceMessenger.Default.Send(
                new AppMessage<StartGameTimerSettings>(startGameTimerSettings));

            RemoveReadyListener();
        }
   
        public override void StartGame()
        {
            if (GameBoard == null || OpGameBoard == null || IsGameStarted) return;

            OpFallTimer = Application.Current!.Dispatcher.CreateTimer();
            OpFallTimer.Interval =
                TimeSpan.FromSeconds(ConstData.OpShapeFallIntervalS);
            OpFallTimer.Tick += ApplyOpMove;

            AddGameListener();
            IsGameStarted = true;
            GameBoard.StartGame();
            OpGameBoard.EnableMoves = true;
        }

        public override void MoveRightShape() 
        {
            if (GameBoard == null) return;
            
            GameBoard.MoveRightShape();
        }
      
        public override void MoveLeftShape() 
        {
            if (GameBoard == null) return;

            GameBoard.MoveLeftShape();
        }
     
        public override async void MoveDownShape() 
        {
            if (GameBoard == null) return;

            await GameBoard.MoveDownShape();
        }
   
        public override void RotateShape() 
        {
            if (GameBoard == null) return;

            GameBoard.RotateShape();
        }
    
        public override void MoveRightOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.MoveRightShape();
        }
   
        public override void MoveLeftOpShape()
        {
            if (OpGameBoard == null) return; 
            OpGameBoard.MoveLeftShape();
        }
     
        public override async void MoveDownOpShape() 
        {
            if (OpGameBoard == null) return;
            await OpGameBoard.MoveDownShape();
        }
     
        public override void RotateOpShape()
        {
            if (OpGameBoard == null) return;
            OpGameBoard.RotateShape();
        }
     
        public override void Ready()
        {
            if (User == null) return;
            fbd.SetPlayerReady(GameID, MaxPlayersCount, User.UserID);
        }

        public override async void CreateCode()
        {
            Random rnd = new();
            int code = 0;
            bool success = false;
            while (!success)
            {
                code = rnd.Next(100000, 1000000);
                success = await fbd.SetPrivateJoinCode(GameID, code);
            }
            PrivateJoinCode = code;
            OnCodeReady?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Protected Methods
        protected override void OnGameFinishedLogicHandler(object? sender, EventArgs e)
        {
            if (GameBoard == null || OpGameBoard == null || GameBoard.User == null || 
                OpGameBoard.User == null || GameBoard.FallTimer == null || OpFallTimer == null) return;
            if (!IsStatsUpdatedOnceOnGameFinished)
            {
                GameBoard.User.HighestScore = GameBoard.User.HighestScore < GameBoard.Score
                    ? GameBoard.Score : GameBoard.User.HighestScore;
                GameBoard.User.GamesPlayed++;
                fbd.UpdateUserPostGame(GameBoard.User);
                IsStatsUpdatedOnceOnGameFinished = true;
            }
            UnregisterTimer();
            GameBoard.FallTimer.Stop();
            OpFallTimer.Stop();
            GameBoard.EnableMoves = false;
            OpGameBoard.EnableMoves = false;
            OnGameFinishedUI?.Invoke(sender as GameBoard, EventArgs.Empty);
        }

        protected override void UnregisterTimer()
        {
            WeakReferenceMessenger.Default.Unregister<AppMessage<long>>(this);
        }
    
        protected override void OnMessageReceived(long timeLeft)
        {
            TimeLeftMs = timeLeft;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnTimeLeftChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    
        protected override void OnChangeReady(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null) return;
            bool allReady = true;
            for (int i = 0; i < MaxPlayersCount && allReady; i++)
            {
                if (!snapshot.Get<bool>(Keys.PlayerDetailsKey + i + 
                    TechnicalConsts.DotSign + Keys.IsPlayerReadyKey))
                    allReady = false;
            }
            if (allReady)
                OnAllReady?.Invoke(this, EventArgs.Empty);
        }
    
        protected override async void OnChangeGame(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null || GameBoard == null || GameBoard.ShapesQueue == null
                || OpGameBoard == null || OpGameBoard.ShapesQueue == null || OpFallTimer == null) return;

            bool found = false;
            for (DesiredIndex = 0; DesiredIndex < MaxPlayersCount && !found; DesiredIndex++)
                if (snapshot.Get<bool>(Keys.PlayerDetailsKey + DesiredIndex + 
                    TechnicalConsts.DotSign + Keys.IsShapeAtBottomKey))
                    found = true;

            if (GameBoard.ShapesQueue.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign 
                + Keys.CurrentShapeInGameIdKey) != GameBoard.ShapesQueue.GetTail().InGameId ||
                OpGameBoard.ShapesQueue.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != OpGameBoard.ShapesQueue.GetTail().InGameId)// Shape has changed
            {
                Shape newShape = fbd.CreateShape(snapshot);
                Shape newShape2 = fbd.CreateShape(snapshot);
                if (GameBoard.ShapesQueue.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != GameBoard.ShapesQueue.GetTail().InGameId)
                    GameBoard.ShapesQueue.Insert(newShape);
                if (OpGameBoard.ShapesQueue.IsEmpty() || snapshot.Get<int>(Keys.CurrentShapeMapKey + TechnicalConsts.DotSign
                + Keys.CurrentShapeInGameIdKey) != OpGameBoard.ShapesQueue.GetTail().InGameId)
                    OpGameBoard.ShapesQueue.Insert(newShape2);
            }
            else if (found && snapshot.Get<string>(Keys.PlayerDetailsKey + (DesiredIndex - 1) + TechnicalConsts.DotSign
                    + Keys.PlayerIdKey) != User.UserID)// Op player has moved
            {
                DesiredIndex--;
                Dictionary<string, string> playerMoveMap = snapshot.Get<Dictionary<string, string>>
                    (Keys.PlayerDetailsKey + DesiredIndex + TechnicalConsts.DotSign + Keys.PlayerMovesKey) ?? [];

                CurrentMovingOpId = snapshot.Get<string>(Keys.PlayerDetailsKey + DesiredIndex + 
                    TechnicalConsts.DotSign + Keys.PlayerIdKey)!;

                fbd.ResetIsShapeAtBottom(GameID, DesiredIndex);

                IsMovesQueueSorting = true;

                foreach (KeyValuePair<string, string> move in playerMoveMap)
                    MovesQueue.Insert(move);
                await MovesQueue.SortByUnixTimestampKeyAsync();
                
                IsMovesQueueSorting = false;

                if (!OpFallTimer.IsRunning)
                    OpFallTimer.Start();
            }
        }
     
        protected override void OnChangeWaitingRoom(IDocumentSnapshot? snapshot, Exception? error)
        {
            if (snapshot == null) return;
            CurrentPlayersCount = snapshot.Get<int>(Keys.CurrentPlayersCountKey);
            fbd.GetPlayersFromDocument(GameID, OnCompleteChange);
        }
  
        protected override void OnCompleteChange(ObservableCollection<User> users)
        {
            UsersInGame.Clear();
            foreach (User user in users) { UsersInGame.Add(user); }

            if (!IsFull)
                OnPlayersChange?.Invoke(this, EventArgs.Empty);
            else
            {
                if (GameBoard == null || OpGameBoard == null || User == null) return;
                fbd.SetGameIsFull(GameID);
                GameBoard.GameID = GameID;
                OpGameBoard.GameID = GameID;
                GameBoard.User = User;
                foreach (User user in UsersInGame)
                    if (user.UserID != User.UserID)
                        OpGameBoard.User = user;
                OnGameFull?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void ApplyOpMove(object? sender, EventArgs e)
        {
            if (GameBoard == null || GameBoard.User == null || OpFallTimer == null) return;
            if (!MovesQueue.IsEmpty() && !IsMovesQueueSorting && GameBoard.User.UserID != CurrentMovingOpId)
            // third check is unnecessary since it enters the second if in the
            // on change game only if its not the player who finished a moved.
            {
                string move = MovesQueue.Remove().Value;
                switch (move)
                {
                    case Keys.RightKey:
                        MoveRightOpShape();
                        break;
                    case Keys.LeftKey:
                        MoveLeftOpShape();
                        break;
                    case Keys.DownKey:
                        MoveDownOpShape();
                        break;
                    case Keys.RotateKey:
                        RotateOpShape();
                        break;
                }
            }
            else
                OpFallTimer.Stop();
        }
        #endregion
    }
}
