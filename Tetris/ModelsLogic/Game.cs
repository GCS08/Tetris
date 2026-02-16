using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using System;
using System.Collections.ObjectModel;
using System.Timers;
using Tetris.Models;
using Tetris.Views;

namespace Tetris.ModelsLogic
{
    public class Game : GameModel
    {
        public Game(string CubeColor, string CreatorName, int CurrentPlayersCount,
        int MaxPlayersCount, bool IsPublicGame, Shape shape, string GameID)
        {
            RegisterTimer();
            this.CubeColor = CubeColor;
            this.CreatorName = CreatorName;
            this.CurrentPlayersCount = CurrentPlayersCount;
            this.MaxPlayersCount = MaxPlayersCount;
            this.IsPublicGame = IsPublicGame;
            this.GameID = GameID;
            this.GameBoard = new(GameID, shape, false);
            this.OpGameBoard = new(GameID, shape.Duplicate(shape), true);
            OpFallTimer.Elapsed += ApplyOpMove;
            OpGameBoard.OnGameFinishedLogic += OnGameFinishedLogicHandler;
        }

        private void OnGameFinishedLogicHandler(object? sender, EventArgs e)
        {
            if (GameBoard == null || OpGameBoard == null) return;
            GameBoard.FallTimer.Stop();
            OpFallTimer.Stop();
            GameBoard.EnableMoves = false;
            OpGameBoard.EnableMoves = false;
            OnGameFinishedUI?.Invoke(this, EventArgs.Empty);
        }

        protected override void RegisterTimer()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }
        protected override void OnMessageReceived(long timeLeft)
        {
            TimeLeftMs = timeLeft;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnTimeLeftChanged?.Invoke(this, EventArgs.Empty);
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
                await fbd.OnPlayerLeaveWR(GameID,
                    (Application.Current as App)!.AppUser.UserID);
                CurrentPlayersCount--;
                UsersInGame.Remove((Application.Current as App)!.AppUser);
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
                || OpGameBoard == null || OpGameBoard.ShapesQueue == null) return;

            bool found = false;
            for (desiredIndex = 0; desiredIndex < MaxPlayersCount && !found; desiredIndex++)
                if (snapshot.Get<bool>(Keys.PlayerDetailsKey + desiredIndex + 
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
            else if (found && snapshot.Get<string>(Keys.PlayerDetailsKey + (desiredIndex - 1) + TechnicalConsts.DotSign
                    + Keys.PlayerIdKey) != (Application.Current as App)!.AppUser.UserID)// Op player has moved
            {
                desiredIndex--;
                Dictionary<string, string> playerMoveMap = snapshot.Get<Dictionary<string, string>>
                    (Keys.PlayerDetailsKey + desiredIndex + TechnicalConsts.DotSign + Keys.PlayerMovesKey) ?? [];

                currentMovingOpId = snapshot.Get<string>(Keys.PlayerDetailsKey + desiredIndex + TechnicalConsts.DotSign + Keys.PlayerIdKey)!;

                fbd.ResetIsShapeAtBottom(GameID, desiredIndex);

                IsMovesQueueSorting = true;

                foreach (KeyValuePair<string, string> move in playerMoveMap)
                    movesQueue.Insert(move);
                await movesQueue.SortByUnixTimestampKeyAsync();
                
                IsMovesQueueSorting = false;

                if (!OpFallTimer.Enabled)
                    OpFallTimer.Start();
            }
        }
        protected override void ApplyOpMove(object? sender, ElapsedEventArgs e)
        {
            if (GameBoard == null || GameBoard.User == null) return;
            if (!movesQueue.IsEmpty() && !IsMovesQueueSorting && GameBoard.User.UserID != currentMovingOpId)
                // third check is unnecessary since it enters the second if in the
                // on change game only if its not the player who finished a moved.
            {
                string move = movesQueue.Remove().Value;
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
                if (GameBoard == null || OpGameBoard == null) return;
                fbd.SetGameIsFull(GameID);
                GameBoard.GameID = GameID;
                OpGameBoard.GameID = GameID;
                GameBoard.User = (Application.Current as App)!.AppUser;
                foreach (User user in UsersInGame)
                    if (user.UserID != (Application.Current as App)!.AppUser.UserID)
                        OpGameBoard.User = user;
                OnGameFull?.Invoke(this, EventArgs.Empty);
            }
        }
        public override async void NavToWR()
        {
            await fbd.OnPlayerJoinWR(GameID,
                (Application.Current as App)!.AppUser.UserID);
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

            AddGameListener();
            IsGameStarted = true;
            GameBoard.StartGame();
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
            fbd.SetPlayerReady(GameID, MaxPlayersCount, (Application.Current as App)!.AppUser.UserID);
        }
    }
}
